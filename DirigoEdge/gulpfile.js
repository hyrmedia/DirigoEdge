var gulp         = require('gulp'),
    sass         = require('gulp-sass'),
    sourcemaps   = require('gulp-sourcemaps'),
    beep         = require('beepbeep'),
    autoprefixer = require('gulp-autoprefixer'),
    rimraf       = require('gulp-rimraf'),
    colors       = require('colors');

gulp.task('sass:dev', function () {

    console.log('[sass]'.bold.magenta + ' Compiling development CSS');

    return gulp.src('scss/main.scss')
        .pipe(sourcemaps.init())
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
        // Should be writing sourcemaps AFTER autoprefixer runs,
        // but that breaks everything right now.
        .pipe(sourcemaps.write())
        .pipe(autoprefixer({
            browsers: ['last 3 versions', 'ie 9']
        }))
        .pipe(gulp.dest('./css'));
});

gulp.task('sass:prod', function () {

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

        .pipe(autoprefixer({
            browsers: ['last 3 versions', 'ie 9']
        }))

        .pipe(gulp.dest('./css'));
});

// Watch files for changes
gulp.task('watch', function () {

    console.log('[watch]'.bold.magenta + ' Watching Sass files for changes');

    gulp.watch(['scss/**/*.scss'], ['sass:dev']);

});

// Compile Sass and watch for file changes
gulp.task('dev', ['sass:dev', 'watch'], function () {
    return console.log('\n[dev]'.bold.magenta + ' Ready for you to start doing things\n'.bold.green);
});

// Compile production Sass
gulp.task('build', ['sass:prod']);

// Visual Studio build tasks
gulp.task('Debug', ['sass:dev']);
gulp.task('Release', ['sass:prod']);