// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Math.Environments
{
    using System.CodeDom.Compiler;
    using Accord.Math;

    /// <summary>
    ///   GNU R algorithm environment. Work in progress.
    /// </summary>
    /// 
    [GeneratedCode("", "")]
    public abstract class REnvironment
    {
        /// <summary>
        ///   Creates a new vector.
        /// </summary>
        /// 
        protected Vec c(params double[] values)
        {
            return values;
        }

        /// <summary>
        ///   Creates a new matrix.
        /// </summary>
        /// 
        protected Mat matrix(double[] values, int rows, int cols)
        {
            return Matrix.Reshape(values, rows, cols, MatrixOrder.FortranColumnMajor);
        }


        /// <summary>
        ///   Placeholder vector definition
        /// </summary>
        /// 
        protected Vec _
        {
            get { return new Vec(null); }
        }


        /// <summary>
        ///   Vector definition operator.
        /// </summary>
        /// 
        protected class Vec
        {
            /// <summary>
            ///   Inner vector object
            /// </summary>
            /// 
            public double[] vector;

            /// <summary>
            ///   Initializes a new instance of the <see cref="Vec"/> class.
            /// </summary>
            /// 
            public Vec(double[] values)
            {
                this.vector = values;
            }

            /// <summary>
            ///   Implements the operator -.
            /// </summary>
            /// 
            public static Vec operator -(Vec v)
            {
                return v;
            }

            /// <summary>
            ///   Implements the operator &lt;.
            /// </summary>
            /// 
            public static Vec operator <(Vec a, Vec v)
            {
                    a.vector = v.vector;
                    return a;
            }

            /// <summary>
            ///   Implements the operator &gt;.
            /// </summary>
            /// 
            public static Vec operator >(Vec a, Vec v)
            {
                return a;
            }

            /// <summary>
            ///   Performs an implicit conversion from <see cref="T:System.Double[]"/>
            ///   to <see cref="Vec"/>.
            /// </summary>
            /// 
            public static implicit operator Vec(double[] v)
            {
                return new Vec(v);
            }


            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="Vec"/> 
            ///   to <see cref="T:System.Double[]"/>.
            /// </summary>
            /// 
            public static implicit operator double[](Vec v)
            {
                return v.vector;
            }
        }

        /// <summary>
        ///   Matrix definition operator.
        /// </summary>
        /// 
        protected class Mat
        {
            /// <summary>
            ///   Inner matrix object.
            /// </summary>
            /// 
            public double[,] matrix;

            /// <summary>
            ///   Initializes a new instance of the <see cref="Mat"/> class.
            /// </summary>
            /// 
            public Mat(double[,] values)
            {
                this.matrix = values;
            }

            /// <summary>
            ///   Implements the operator -.
            /// </summary>
            /// 
            public static Mat operator -(Mat v)
            {
                return v;
            }

            /// <summary>
            ///   Implements the operator &lt;.
            /// </summary>
            /// 
            public static Mat operator <(Mat a, Mat v)
            {
                a.matrix = v.matrix;
                return a;
            }

            /// <summary>
            ///    Implements the operator &gt;.
            /// </summary>
            /// 
            public static Mat operator >(Mat a, Mat v)
            {
                return a;
            }

            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="T:System.Double[]"/> to 
            ///   <see cref="Mat"/>.
            /// </summary>
            /// 
            public static implicit operator Mat(double[,] v)
            {
                return new Mat(v);
            }


            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="Mat"/> 
            ///   to <see cref="T:System.Double[]"/>.
            /// </summary>
            /// 
            public static implicit operator double[,](Mat v)
            {
                return v.matrix;
            }
        }
    }
}
