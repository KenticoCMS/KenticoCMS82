cmsdefine(['CMS.OnlineMarketing/ContactImport/FileStreamer'], function (FileStreamer) {

    describe('read', function () {
        var streamer,
            testString = 'Oliver,Queen,The Hood,Green Arrow,USA\nSlade,Wilson,Deathstroke,Australia\nJohn,Diggle,USA\n';

        beforeEach(function () {
            var blob = new Blob([testString]);
            streamer = new FileStreamer(blob);
        });

        it('reads bulks of default size', function (done) {
            streamer.read(function (bulk) {
                var lines = bulk.split(/\r\n|\r|\n/g);

                expect(lines[0]).toBe('Oliver,Queen,The Hood,Green Arrow,USA');
                expect(lines[1]).toBe('Slade,Wilson,Deathstroke,Australia');
                done();
            });
        });

        it('reads small bulks', function (done) {
            var firstRowRead = false;

            // Choose bulk size smaller than the size of one row
            streamer.config = {};
            streamer.config.bulkSize = 5;

            streamer.read(function (bulk, finished) {
                if (!firstRowRead) {
                    firstRowRead = true;
                    expect(bulk).toBe('Oliver,Queen,The Hood,Green Arrow,USA');
                }
                if (finished) {
                    done();
                }
            });
        });
    });
});