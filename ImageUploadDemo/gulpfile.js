/// <binding AfterBuild='postBuild' />

var gulp = require("gulp");

var paths = {
    jsdeps: "./jsdeps",
    npmSrc: "./node_modules/",
    libTarget: "./wwwroot/libs/"
};

var libsToMove = [
   paths.npmSrc + '/angular2/bundles/angular2-polyfills.js',
   paths.npmSrc + '/systemjs/dist/system-polyfills.js',
   paths.npmSrc + '/systemjs/dist/system.js',
   paths.npmSrc + '/rxjs/bundles/Rx.js',
   paths.npmSrc + '/angular2/bundles/angular2.js',
   paths.npmSrc + '/es6-shim/es6-shim.js',
   paths.npmSrc + '/bootstrap/dist/js/bootstrap.js',
   paths.npmSrc + '/bootstrap/dist/css/bootstrap.css',
   paths.npmSrc + '/bootstrap/dist/css/bootstrap.css',

   paths.jsdeps + "/jquery.fileupload/*",
];

gulp.task('postBuild', ["moveToLibs", 'moveToFonts'])
gulp.task('moveToLibs', function () {
    return gulp.src(libsToMove).pipe(gulp.dest(paths.libTarget));
});

gulp.task('moveToFonts', function () {
    return gulp.src([paths.npmSrc + "/bootstrap/fonts/*"]).pipe(gulp.dest("./wwwroot/fonts"));
});