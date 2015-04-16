cmsdefine(['angular', 'angularMocks', 'CMS/Filters/StringFormat'], function (angular, mocks, functionThatReturnsAngularModuleName) {

    describe('StringHelper', function () {
        var filterModuleName = functionThatReturnsAngularModuleName(),
            filter;

        beforeEach(mocks.module(filterModuleName));

        beforeEach(mocks.inject(function ($filter) {
            filter = $filter("stringFormat");
        }));

        it('fills placeholders with correct values', function () {
            expect(filter("Oliver {0} = {1} {2}", "Queen", "Green", "Arrow")).toBe("Oliver Queen = Green Arrow");
        });

        it('fills in undefined for missing values', function () {
            expect(filter("Oliver {0} = {1} {2}", "Queen", "Green")).toBe("Oliver Queen = Green undefined");
        });

        it('ignores extra values', function () {
            expect(filter("Oliver {0} = {1} Arrow", "Queen", "Green", "The Hood")).toBe("Oliver Queen = Green Arrow");
        });
    });
});