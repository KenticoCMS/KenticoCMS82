var cmsdefine = define;
var allTestFiles = [];
var TEST_REGEXP = /(Spec|Tests)\.js$/i;

var pathToModule = function(path) {
    return path.replace(/^\/base\/CMSModules\//, '').replace(/^\/base\/CMSTests\//, '..\/CMSTests\/').replace(/\.js$/, '');
};

Object.keys(window.__karma__.files).forEach(function(file) {
  if (TEST_REGEXP.test(file)) {
    // Normalize paths to RequireJS module names.
    allTestFiles.push(pathToModule(file));
  }
});

require.config({
   

  // Karma serves files under /base, which is the basePath from your config file
    baseUrl: '/base/CMSModules/',

  // dynamically load all test files
  deps: allTestFiles,

  paths: {
      'Underscore': '../Underscore/underscore.min',
      'd3-dsv': '../Vendor/d3-v3.4.11/d3.dsv',

      'angular': '../Vendor/Angular/angular',
      'angularMocks': '../Vendor/Angular/angular-mocks',

      'csv-parser': '../Vendor/CSV-JS/csv',
  },
  shim: {
      'Underscore': {
          exports: '_'
      },
      'angular': {
          exports: 'angular'
      },
      'angularMocks': {
          deps: ['angular'],
          exports: 'angular.mock'
      },
  },

  // we have to kickoff jasmine, as it is asynchronous
  callback: window.__karma__.start
});