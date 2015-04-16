cmsdefine(['CMS.OnlineMarketing/ContactImport/CSVParser'], function (Parser) {
    describe('parseRows', function () {
        it('parses rows', function () {
            // Arrange
            var lines = [
                    'Oliver,Queen,The Hood,Green Arrow,USA',
                    'Slade,Wilson,Deathstroke,Australia',
                    'John,Diggle,USA,',
                    ',',
                    ','
                ],
                result;

            // Act
            result = Parser.parseRows(lines);

            // Assert
            expect(result[0].length).toBe(5);
            expect(result[0][0]).toBe('Oliver');
            expect(result[0][4]).toBe('USA');

            expect(result[2].length).toBe(4);
            expect(result[2][0]).toBe('John');
            expect(result[2][1]).toBe('Diggle');

            expect(result[3].length).toBe(2);
            expect(result[4].length).toBe(2);
        });
    });

    describe('parseBulk', function () {
        it('parses bulk', function () {
            // Arrange
            var bulk =  'Oliver,Queen,The Hood,Green Arrow,USA\r\nSlade,Wilson,Deathstroke,Australia\r\nJohn,Diggle,USA\r\n',
                result;

            // Act
            result = Parser.parseBulk(bulk, false);

            // Assert
            expect(result[0].length).toBe(5);
            expect(result[0][0]).toBe('Oliver');
            expect(result[0][3]).toBe('Green Arrow');

            expect(result[1].length).toBe(4);

            expect(result[2].length).toBe(3);
            expect(result[2][0]).toBe('John');
            expect(result[2][1]).toBe('Diggle');
            expect(result[2][3]).toBeUndefined();
        });

        it('excludes first row', function() {
            // Arrange
            var bulk = 'First name,Last name,City\r\nOliver,Queen,Starling\r\nBruce,Wayne,Gotham',
                result;

            // Act
            result = Parser.parseBulk(bulk, true);

            // Assert
            expect(result[0].length).toBe(3);
            expect(result[0][0]).toBe('Oliver');
        });
    });
    
    describe('filterColumns', function () {
        it('filters columns', function () {
            // Arrange
            var includedColumns = [0, 3, 4],
                parsedLines = [
                    ['Oliver', 'Queen', 'The Hood', 'Green Arrow', 'USA'],
                    ['Slade', 'Wilson', 'Deathstroke', 'Australia'],
                    ['John', 'Diggle', 'USA'],
                    []
                ],
                result;

            // Act
            result = Parser.filterColumns(parsedLines, includedColumns);

            // Assert
            result.forEach(function (line) {
                expect(line.length).toBe(includedColumns.length);
            });

            expect(result[0][0]).toBe('Oliver');
            expect(result[0][1]).toBe('Green Arrow');

            expect(result[2][1]).toBeUndefined();
            expect(result[2][2]).toBeUndefined();
        });
    });

    describe('fixLineColumnsCount', function () {
        it('fixes lines', function () {
            // Arrange
            var columnCount = 4,
                parsedLines = [
                    ['Oliver', 'Queen', 'The Hood', 'Green Arrow', 'USA'],
                    ['Slade', 'Wilson', 'Deathstroke', 'Australia'],
                    ['John', 'Diggle', 'USA'],
                    []
                ];

            // Act
            Parser.fixLineColumnsCount(parsedLines, columnCount);

            // Assert
            parsedLines.forEach(function (line) {
                expect(line.length).toBe(columnCount);
            });

            expect(parsedLines[0][columnCount]).toBeUndefined();
            expect(parsedLines[2][columnCount - 1]).toBe('');
            expect(parsedLines[3][0]).toBe('');
            expect(parsedLines[3][columnCount - 1]).toBe('');
        });
    });

    describe('checkLineLength', function () {
        it('returns true for same-length lines', function () {
            var parsedLines = [['Oliver', 'Queen', 'Arrow', 'USA'], ['Slade', 'Wilson', 'Deathstroke', 'Australia']];
            expect(Parser.checkLineLength(parsedLines)).toBe(true);
        });
    });
});