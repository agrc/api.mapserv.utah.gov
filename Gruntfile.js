module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        cssmin: {
            target: {
                files: {
                    'WebAPI.API/Content/Site.min.css': 'WebAPI.API/Content/Site.css'
                }
            }
        },
        uglify: {
            options: {
                preserveComments: false,
                compress: {
                    drop_console: true,
                    passes: 5,
                    dead_code: true
                }
            },
            main: {
                files: {
                    'WebAPI.API/Scripts/site.min.js': 'WebAPI.API/Scripts/site.js',
                    'WebAPI.API/Scripts/scrollspy.min.js': 'WebAPI.API/Scripts/scrollspy.js',
                    'WebAPI.API/Scripts/modernizr.min.js': 'WebAPI.API/Scripts/modernizr-2.6.1.js',
                    'WebAPI.API/Scripts/jquery.min.js': 'WebAPI.API/Scripts/jquery-1.10.1.js'
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');

    grunt.registerTask('default', ['uglify', 'cssmin']);
};