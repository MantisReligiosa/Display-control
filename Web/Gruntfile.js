/// <binding BeforeBuild='all' ProjectOpened='all' />
module.exports = function (grunt) {
    require('load-grunt-tasks')(grunt);
    grunt.initConfig({
        clean: ["Script/Vendor/"],
        uglify: {
            my_target: {
                files: {
                    'Script/Vendor/vendor.js':
                        [
                            'node_modules/jquery/dist/jquery.min.js',
                            'node_modules/knockout/build/output/knockout-latest.js',
                            'node_modules/popper.js/dist/umd/popper.js',
                            'node_modules/bootstrap/dist/js/bootstrap.js',
                            'node_modules/toastr/toastr.js',
                            'node_modules/interactjs/dist/interact.min.js',
                            'node_modules/bootstrap-colorpicker/dist/js/bootstrap-colorpicker.min.js',
                            'node_modules/bootstrap-multiselect/dist/js/bootstrap-multiselect.js',
                            'node_modules/moment/min/moment-with-locales.min.js',
                            'node_modules/jstree/dist/jstree.min.js',
                            'node_modules/jquery-datetimepicker/build/jquery.datetimepicker.full.min.js'
                        ]
                }
            }
        },
        cssmin: {
            options: {
                mergeIntoShorthands: false,
                roundingPrecision: -1
            },
            target: {
                files: {
                    'css/vendor.css': [
                        'node_modules/bootstrap/dist/css/bootstrap.css',
                        'node_modules/toastr/build/toastr.min.css',
                        'node_modules/bootstrap-colorpicker/dist/css/bootstrap-colorpicker.min.css',
                        'node_modules/bootstrap-multiselect/dist/css/bootstrap-multiselect.css',
                        'node_modules/jstree-bootstrap-theme/dist/themes/proton/style.min.css',
                        'node_modules/jquery-datetimepicker/jquery.datetimepicker.css',
                        'node_modules/jquery-datetimepicker/build/jquery.datetimepicker.full.min.css'
                        //'node_modules/jstree/dist/themes/default/style.min.css'
                    ]
                }
            }
        },
        copy: {
            main: {
                files: [
                    {
                        expand: true,
                        flatten: true,
                        src: [
                            //'node_modules/jstree/dist/themes/default/*.png',
                            'node_modules/jstree-bootstrap-theme/dist/themes/proton/*.png',
                            'node_modules/jstree/dist/themes/default/*.gif'
                        ],
                        dest: 'assets/',
                        filter: 'isFile'
                    },
                ],
            },
        },
    });
    grunt.registerTask("all", ['clean', 'uglify', 'cssmin', 'copy']);
};