cmsdefine(['CMS/StringHelper'], function (StringHelper) {
    var testStringWindows = 'My name is Oliver Queen.\r\nFor five years I was stranded on an island with only one goal.\r\nSurvive.',
        testStringLinux = 'My name is Oliver Queen.\nFor five years I was stranded on an island with only one goal.\nSurvive.';

    describe('getLastEndOfLinePosition', function () {
        it('gets last end of line rn', function () {
            var lastEndOfLine = StringHelper.getLastEndOfLinePosition(testStringWindows);
            expect(lastEndOfLine).toEqual({
                start: 88,
                end: 89
            });

        });

        it('gets last end of line n', function () {
            var lastEndOfLine = StringHelper.getLastEndOfLinePosition(testStringLinux);
            expect(lastEndOfLine).toEqual({
                start: 87,
                end: 87
            });
        });
    });
});