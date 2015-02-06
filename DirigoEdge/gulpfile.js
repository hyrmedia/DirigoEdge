var gulp = require('gulp'),
    sass = require('gulp-sass'),
    sourcemaps = require('gulp-sourcemaps'),
    beep = require('beepbeep'),
    prefix = require('gulp-autoprefixer'),
    rimraf = require('gulp-rimraf'),
    colors = require('colors');

gulp.task('sass:dev', ['clean'], function () {

    console.log('[sass]'.bold.magenta + ' Compiling development CSS');

    return gulp.src('scss/main.scss')
        .pipe(sass({
            outputStyle: 'expanded',
            sourceMap: true
        }))

        .on('error', function (error) {
            beep();
            console.log('[sass]'.bold.magenta + ' There was an issue compiling Sass'.bold.red);
            console.error(error.message);
            this.emit('end');
        })

        .pipe(sourcemaps.init({ loadMaps: true }))

        .pipe(prefix({
            browsers: ['last 3 versions', 'ie 9']
        }))

        .pipe(sourcemaps.write())

        .pipe(gulp.dest('./css'));
});

gulp.task('sass:prod', ['clean'], function () {

    console.log('[sass]'.bold.magenta + ' Compiling production CSS');

    return gulp.src('scss/main.scss')

        .pipe(sass({
            outputStyle: 'compressed',
            sourcemap: false
        }))

        .on('error', function (error) {
            beep();
            console.error(error);
            this.emit('end');
        })

        .pipe(prefix({
            browsers: ['last 3 versions', 'ie 9']
        }))

        .pipe(gulp.dest('./css'));
});

// Delete compiled for development
gulp.task('clean', function () {

    console.log('[clean]'.bold.magenta + ' Deleting compiled CSS files');

    return gulp.src(['css/**/*{.css,.css.map}'], { read: false })
        .pipe(rimraf());

});

// Watch files for changes
gulp.task('watch', function () {

    console.log('[watch]'.bold.magenta + ' Watching Sass files for changes');

    gulp.watch(['scss/**/*.scss'], ['sass:dev']);

});

// Development task
gulp.task('dev', ['sass:dev', 'watch'], function () {
    return console.log('\n[dev]'.bold.magenta + ' Ready for you to start doing things\n'.bold.green);
});

// Production task
gulp.task('build', ['sass:prod']);

// Visual Studio build tasks
gulp.task('Debug', ['sass:dev']);
gulp.task('Release', ['sass:prod']);