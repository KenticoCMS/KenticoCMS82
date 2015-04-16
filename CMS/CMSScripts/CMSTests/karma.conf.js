// Karma configuration

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '../',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine', 'requirejs'],


    // list of files / patterns to load in the browser
    files: [
        { pattern: 'Underscore/underscore.min.js', included: false },
        { pattern: 'Vendor/Angular/angular.js', included: false },
        { pattern: 'Vendor/Angular/angular-mocks.js', included: false },

        { pattern: 'CMSModules/CMS/StringHelper.js', included: false },
        { pattern: 'CMSTests/CMS/StringHelperTests.js', included: false },
    
//    JShints complaint / jasmine error: this.results_ is null
//        { pattern: 'CMSModules/CMS.OnlineMarketing/ContactImport/FileStreamer.js', included: false },
//        { pattern: 'CMSTests/CMS.OnlineMarketing/ContactImport/FileStreamerTests.js', included: false },

//    'There is no timestamp for /base/CMSModules/csv-parser.js!'     /     404: /base/CMSModules/csv-parser.js
//        { pattern: 'CMSModules/CMS.OnlineMarketing/ContactImport/CSVParser.js', included: false },
//        { pattern: 'CMSTests/CMS.OnlineMarketing/ContactImport/CSVParserTests.js', included: false },

        
//    'There is no timestamp for /base/CMSModules/CMS/StringFormatter.js!'     /     404: /base/CMSModules/CMS/StringFormatter.js
//        { pattern: 'CMSModules/CMS/Filters/StringFormat.js', included: false },
//        { pattern: 'CMSTests/CMS/Filters/StringFormatTests.js', included: false },

    'CMSTests/js-test-main.js'
    ],


    // list of files to exclude
    exclude: [
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        'CMSModules/CMS.OnlineMarketing/ContactImport/*': ['jshint'],
        'CMSTests/CMS*/**/*': ['jshint'],
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: false,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Firefox'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
