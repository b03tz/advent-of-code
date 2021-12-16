namespace AdventOfCode.Helpers
{
    public static class ArrayHelpers
    {
        public static TType[,] CutArrayX<TType>(this TType[,] input, int offset, int limit)
        {
            var newArray = new TType[input.GetLength(0), limit];

            for (var row = 0; row < input.GetLength(0); row++)
            {
                var newColumn = 0;
                for (var column = offset; column < offset + limit; column++)
                {
                    newArray[row, newColumn] = input[row, column];
                    newColumn++;
                }
            }

            return newArray;
        }
        
        public static TType[,] CutArrayY<TType>(this TType[,] input, int offset, int limit)
        {
            var newArray = new TType[limit, input.GetLength(1)];

            var newRow = 0;
            for (var row = offset; row < offset + limit; row++)
            {
                for (var column = 0; column < newArray.GetLength(1); column++)
                    newArray[newRow, column] = input[row, column];

                newRow++;
            }

            return newArray;
        }
        
        public static TType[,] RepeatArrayColumns<TType>(this TType[,] input, int howManyColumns = 11)
        {
            var newArray = new TType[input.GetLength(0), input.GetLength(1) + howManyColumns];

            for (var row = 0; row < newArray.GetLength(0); row++)
            {
                for (var column = 0; column < newArray.GetLength(1); column++)
                {
                    var actualColumn = column;
                    if (column >= input.GetLength(1))
                        actualColumn = column - howManyColumns;
                    
                    newArray[row, column] = input[row, actualColumn];
                }
            }

            return newArray;
        }
        
        public static TType[,] RepeatArrayRows<TType>(this TType[,] input, int howManyRows = 11)
        {
            var newArray = new TType[input.GetLength(0), input.GetLength(1) + howManyRows];

            for (var row = 0; row < newArray.GetLength(0); row++)
            {
                var actualRow = row;
                if (row >= input.GetLength(1))
                    actualRow = row - howManyRows;
                
                for (var column = 0; column < newArray.GetLength(1); column++)
                    newArray[row, column] = input[actualRow, column];
            }

            return newArray;
        }
    }
}