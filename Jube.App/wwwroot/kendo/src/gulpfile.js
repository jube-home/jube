/* jshint browser:false, node:true, esnext: true */

var gulp = require('gulp');
var logger = require('gulp-logger');
var filter = require('gulp-filter');
var util = require('gulp-util');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var lazypipe = require('lazypipe');
var uglify = require('gulp-uglify');
var rename = require('gulp-rename');
var replace = require('gulp-replace');
var foreach = require('gulp-foreach');
var amdOptimize = require("amd-optimize");
var argv = require('yargs').argv;

var less = require('gulp-less');
var cleanCss = require('gulp-clean-css');
var clone = require('gulp-clone');
var gulpIf = require('gulp-if');
var merge = require('merge2');

var makeSourceMaps = !argv['skip-source-maps'];

var postcss = require("gulp-postcss");
var autoprefixer = require("autoprefixer");
var calc = require("postcss-calc");
var browsers = [
    "Explorer >= 9",
    "last 3 Edge versions",
    "last 2 Chrome versions",
    "last 2 Firefox versions",
    "last 2 Opera versions",
    "last 2 Safari major versions",
    "last 2 iOS major versions",
    "Android >= 4.4",
    "ExplorerMobile >= 10"
];
var postcssPlugins = [
    calc({
        precision: 10
    }),
    autoprefixer({
        browsers: browsers
    })
];

var cleanCssOptions = {
    compatibility: 'ie9',
    aggressiveMerging: false,
    advanced: false
};

var fromLess = lazypipe()
    .pipe(logger, { after: 'LESS complete!', extname: '.css', showChange: true })
    .pipe(less, {
        math: 'strict',
        relativeUrls: true,
        plugins: []
    })
    .pipe(replace, /\.\.\/mobile\//g, ''); // temp hack for the discrepancy between source and generated "source"

var minify = lazypipe()
    .pipe(logger, { after: 'Min CSS complete!', extname: '.min.css', showChange: true } )
    .pipe(cleanCss, cleanCssOptions)
    .pipe(rename, { suffix: ".min" });

var argv = require('yargs').argv;

// uglify
var compress = {
    unsafe       : true,
    hoist_vars   : true,
    warnings     : false,
    pure_getters : true
};

var mangle = {
    except: [ "define" ]
};

function renameModules(match) {
    return match.replace(/['"]([\w\.\-\/]+)?['"]/g, function(_, module) {
        return module == "jquery" ? '"jquery"' : `"${module}.min"`
    });
}

var uglify = lazypipe()
    .pipe(logger, { after: 'uglify complete', extname: '.min.js', showChange: true })
    .pipe(uglify, { compress, mangle, preserveComments: "license" })
    .pipe(replace, /define\(".+?\]/g, renameModules)
    .pipe(rename, { suffix: ".min" });


// AMD gathering

function gatherCustomAmd(stream, file) {
    var moduleId = file.path.match(/kendo\.(.+)\.js/)[1];

    console.log(moduleId);
    return stream.pipe(amdOptimize(`kendo.${moduleId}`, {
        baseUrl: "js",
        exclude: [ 'jquery' ]
    }));
}

var gatherCustom = lazypipe()
    .pipe(foreach, gatherCustomAmd);

gulp.task("custom", function() {
    var files = argv.c;

    if (!files) {
        throw new util.PluginError({
            task: "custom",
            plugin: "custom",
            message: "please provide a list of the components to be included in the build with -c, separated with ','"
        });
    }

    if (files.indexOf(',') !== -1) {
        files = `{${files}}`;
    }

    var included = [];
    return gulp.src(`js/kendo.${files}.js`, { base: "js" })
        .pipe(gatherCustom())
        .pipe(filter(function(file) {
            if (included.indexOf(file.path) === -1) {
                included.push(file.path);
                return true;
            }  else {
                return false;
            }
        }))
        .pipe(concat({path: 'js/kendo.custom.js', base: 'js'}))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(logger({after: 'source map complete!', extname: '.map', showChange: true}))
        .pipe(sourcemaps.write("./"))
        .pipe(gulp.dest("dist/js"));
});

gulp.task("less",function() {
    var css = gulp.src(`styles/${argv.styles || '**/kendo*.less'}`, { base: "styles" })
        .pipe(fromLess())
        .pipe(postcss(postcssPlugins));

    var minCss = css.pipe(clone())
        .pipe(gulpIf(makeSourceMaps, sourcemaps.init()))
        .pipe(minify())
        .pipe(gulpIf(makeSourceMaps, sourcemaps.write("./")));

    return merge(css, minCss)
        .pipe(gulp.dest('dist/styles'));
});
