var schedulerTasks = [
    {
        id: 1,
        title: "AP Physics",
        image: "../content/web/scheduler/physics.png",
        start: new Date("2020/10/5 8:00"),
        end: new Date("2020/10/5 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=MO;WKST=SU",
        attendee: 1
    },
    {
        id: 2,
        title: "History",
        image: "../content/web/scheduler/history.png",
        start: new Date("2020/10/5 9:00"),
        end: new Date("2020/10/5 10:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=9;BYDAY=MO,WE,TH,FR;WKST=SU",
        attendee: 1
    },
    {
        id: 3,
        title: "Art",
        image: "../content/web/scheduler/art.png",
        start: new Date("2020/10/5 9:00"),
        end: new Date("2020/10/5 10:00"),
        attendee: 2
    },
    {
        id: 4,
        title: "Spanish",
        image: "../content/web/scheduler/spanish.png",
        start: new Date("2020/10/5 10:00"),
        end: new Date("2020/10/5 11:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=MO,TH;WKST=SU",
        attendee: 1
    },
    {
        id: 5,
        title: "Home Ec",
        image: "../content/web/scheduler/home-ec.png",
        start: new Date("2020/10/5 10:00"),
        end: new Date("2020/10/5 11:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=MO,TH;WKST=SU",
        attendee: 2
    },
    {
        id: 6,
        title: "AP Math",
        image: "../content/web/scheduler/math.png",
        start: new Date("2020/10/5 11:00"),
        end: new Date("2020/10/5 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=MO,TH;WKST=SU",
        attendee: 1
    },
    {
        id: 7,
        title: "AP Econ",
        image: "../content/web/scheduler/econ.png",
        start: new Date("2020/10/5 11:00"),
        end: new Date("2020/10/5 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=MO,TH;WKST=SU",
        attendee: 2
    },
    {
        id: 8,
        title: "Photography Club Meeting",
        image: "../content/web/scheduler/photography.png",
        start: new Date("2020/10/5 14:00"),
        end: new Date("2020/10/5 15:30"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=MO;WKST=SU",
        attendee: 2
    },
    {
        id: 9,
        title: "Tennis Practice",
        image: "../content/web/scheduler/tennis.png",
        start: new Date("2020/10/5 15:30"),
        end: new Date("2020/10/5 16:30"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=MO;WKST=SU",
        attendee: 1
    },
    {
        id: 10,
        title: "French",
        image: "../content/web/scheduler/french.png",
        start: new Date("2020/10/6 8:00"),
        end: new Date("2020/10/6 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TU;WKST=SU",
        attendee: 2
    },
    {
        id: 11,
        title: "Gym",
        image: "../content/web/scheduler/gym.png",
        start: new Date("2020/10/6 9:00"),
        end: new Date("2020/10/6 10:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=TU;WKST=SU",
        attendee: 1
    },
    {
        id: 12,
        title: "English",
        image: "../content/web/scheduler/english.png",
        start: new Date("2020/10/6 9:00"),
        end: new Date("2020/10/6 10:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TU;WKST=SU",
        attendee: 2
    },
    {
        id: 13,
        title: "English",
        image: "../content/web/scheduler/english.png",
        start: new Date("2020/10/6 10:00"),
        end: new Date("2020/10/6 11:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=12;BYDAY=TU,FR;WKST=SU",
        attendee: 1
    },
    {
        id: 14,
        title: "History",
        image: "../content/web/scheduler/history.png",
        start: new Date("2020/10/6 11:00"),
        end: new Date("2020/10/6 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TU;WKST=SU",
        attendee: 1
    },
    {
        id: 15,
        title: "Gym",
        image: "../content/web/scheduler/gym.png",
        start: new Date("2020/10/6 11:00"),
        end: new Date("2020/10/6 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TU;WKST=SU",
        attendee: 2
    },
    {
        id: 16,
        title: "English",
        image: "../content/web/scheduler/english.png",
        start: new Date("2020/10/6 8:00"),
        end: new Date("2020/10/6 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=WE;WKST=SU",
        attendee: 1
    },
    {
        id: 17,
        title: "School Choir Practice",
        image: "../content/web/scheduler/choir.png",
        start: new Date("2020/10/6 14:30"),
        end: new Date("2020/10/6 15:30"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TU;WKST=SU",
        attendee: 2
    },
    {
        id: 18,
        title: "Art",
        image: "../content/web/scheduler/art.png",
        start: new Date("2020/10/7 8:00"),
        end: new Date("2020/10/7 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=WE;WKST=SU",
        attendee: 2
    },
    {
        id: 19,
        title: "French",
        image: "../content/web/scheduler/french.png",
        start: new Date("2020/10/7 9:00"),
        end: new Date("2020/10/7 10:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=10;BYDAY=WE,FR;WKST=SU",
        attendee: 2
    },
    {
        id: 20,
        title: "Gym",
        image: "../content/web/scheduler/gym.png",
        start: new Date("2020/10/7 10:00"),
        end: new Date("2020/10/7 11:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=WE;WKST=SU",
        attendee: 1
    },
    {
        id: 21,
        title: "English",
        image: "../content/web/scheduler/english.png",
        start: new Date("2020/10/7 10:00"),
        end: new Date("2020/10/7 11:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=WE;WKST=SU",
        attendee: 2
    },
    {
        id: 22,
        title: "AP Physics",
        image: "../content/web/scheduler/physics.png",
        start: new Date("2020/10/7 11:00"),
        end: new Date("2020/10/7 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=WE;WKST=SU",
        attendee: 1
    },
    {
        id: 23,
        title: "Spanish Club",
        image: "../content/web/scheduler/spanish.png",
        start: new Date("2020/10/7 13:30"),
        end: new Date("2020/10/7 15:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=WE;WKST=SU",
        attendee: 1
    },
    {
        id: 24,
        title: "AP CompSci",
        image: "../content/web/scheduler/computer-science.png",
        start: new Date("2020/10/8 8:00"),
        end: new Date("2020/10/8 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TH;WKST=SU",
        attendee: 1
    },
    {
        id: 25,
        title: "Gym",
        image: "../content/web/scheduler/gym.png",
        start: new Date("2020/10/8 9:00"),
        end: new Date("2020/10/8 10:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TH;WKST=SU",
        attendee: 2
    },
    {
        id: 26,
        title: "School Paper Meeting",
        image: "../content/web/scheduler/newspaper.png",
        start: new Date("2020/10/8 14:00"),
        end: new Date("2020/10/8 15:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TH;WKST=SU",
        attendee: 2
    },
    {
        id: 27,
        title: "Tennis Practice",
        image: "../content/web/scheduler/tennis.png",
        start: new Date("2020/10/8 15:00"),
        end: new Date("2020/10/8 16:30"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=TH;WKST=SU",
        attendee: 2
    },
    {
        id: 28,
        title: "English",
        image: "../content/web/scheduler/english.png",
        start: new Date("2020/10/9 8:00"),
        end: new Date("2020/10/9 9:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=FR;WKST=SU",
        attendee: 2
    },
    {
        id: 29,
        title: "AP CompSci",
        image: "../content/web/scheduler/computer-science.png",
        start: new Date("2020/10/9 11:00"),
        end: new Date("2020/10/9 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=FR;WKST=SU",
        attendee: 1
    },
    {
        id: 30,
        title: "Art",
        image: "../content/web/scheduler/art.png",
        start: new Date("2020/10/9 11:00"),
        end: new Date("2020/10/9 12:00"),
        recurrenceRule: "FREQ=WEEKLY;COUNT=5;BYDAY=FR;WKST=SU",
        attendee: 2
    },
];