/// <vs SolutionOpened='dev, dev:admin' />
var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var beep = require('beepbeep');
var autoprefixer = require('gulp-autoprefixer');
var rimraf = require('gulp-rimraf');
var colors = require('colors');
var livereload = require('gulp-livereload');
var handlebars = require('gulp-handlebars');
var wrap = require('gulp-wrap');
var declare = require('gulp-declare');
var concat = require('gulp-concat');


gulp.task('sass:dev', function () {

    console.log('[sass]'.bold.magenta + ' Compiling development CSS');

    return gulp.src('scss/*.scss')
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
        .pipe(gulp.dest('./css'))
        .pipe(livereload());
});

gulp.task('sass:prod', function () {

    console.log('[sass]'.bold.magenta + ' Compiling production CSS');

    return gulp.src('scss/*.scss')

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

gulp.task('sass:admin:dev', function () {

    console.log('[sass]'.bold.magenta + ' Compiling development CSS');

    return gulp.src('areas/admin/scss/*.scss')
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

        .pipe(gulp.dest('./areas/admin/css'))
        .pipe(livereload());
        
});

gulp.task('sass:admin:prod', function () {

    console.log('[sass]'.bold.magenta + ' Compiling production CSS');

    return gulp.src('areas/admin/scss/*.scss')

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

        .pipe(gulp.dest('./areas/admin/css'));
});

// Compile handlebars templates for schemas
gulp.task('schema-templates', function () {

    console.log('[handlebars]'.bold.magenta + ' Compiling handlebars templates for schemas');

    gulp.src('areas/admin/scripts/schema/templates/**/*.hbs')
        .pipe(handlebars({
            handlebars: require('handlebars')
        }))
        .pipe(wrap('Handlebars.template(<%= contents %>)'))
        .pipe(declare({
            namespace: 'Schema.templates',
            noRedeclare: true,
            processName: function (filePath) {
                return declare.processNameByPath(filePath.replace('areas\\admin\\scripts\\schema\\templates\\', ''));
            }
        }))
        .pipe(concat('templates.js'))
        .pipe(gulp.dest('areas/admin/scripts/schema/templates/'));

});

// Watch files for changes
gulp.task('watch', function () {

    console.log('[watch]'.bold.magenta + ' Watching files for changes');

    livereload.listen();
    gulp.watch(['scss/**/*.scss'], ['sass:dev']);

});

// Watch files for changes
gulp.task('watch:admin', function () {

    console.log('[watch]'.bold.magenta + ' Watching files for changes');

    livereload.listen();
    gulp.watch(['areas/admin/scss/**/*.scss'], ['sass:admin:dev']);
    gulp.watch(['areas/admin/scripts/schema/templates/**/*.hbs'], ['schema-templates']);

});

// Compile Sass and watch for file changes
gulp.task('dev', ['sass:dev', 'watch'], function () {
    return console.log('\n[dev]'.bold.magenta + ' Ready for you to start doing things\n'.bold.green);
});

gulp.task('dev:admin', ['sass:admin:dev', 'schema-templates', 'watch:admin'], function () {
    return console.log('\n[dev]'.bold.magenta + ' Ready for you to start doing things\n'.bold.green);
});

// Compile production Sass
gulp.task('build', ['sass:prod', 'sass:admin:prod']);

// Visual Studio build tasks
gulp.task('Debug', ['sass:dev', 'sass:admin:dev']);
gulp.task('Release', ['sass:prod', 'sass:admin:prod']);