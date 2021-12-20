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
        
        public static TType[,] PadArray<TType>(this TType[,] inputArray, TType padValue, int paddingSize)
        {
            paddingSize = paddingSize * 2;
            var newArray = new TType[inputArray.GetLength(0) + paddingSize, inputArray.GetLength(1) + paddingSize];

            for (var row = 0; row < inputArray.GetLength(0) + paddingSize; row++)
            for (var col = 0; col < inputArray.GetLength(1) + paddingSize; col++)
            {
                if (row < (paddingSize / 2) || 
                    (row - (paddingSize / 2)) > inputArray.GetLength(0) - 1 || 
                    col < (paddingSize / 2) || 
                    (col - (paddingSize / 2)) > inputArray.GetLength(1) - 1)
                {
                    newArray[row, col] = padValue;
                    continue;
                }

                newArray[row, col] = inputArray[row - (paddingSize / 2), col - (paddingSize / 2)];
            }
            
            return newArray;
        }
        
        public static TType? GetFromArray<TType>(this TType[,] input, int row, int col)
        where TType : class
        {
            if (row < 0 || row > input.GetLength(0) - 1)
                return null;

            if (col < 0 || col > input.GetLength(1) - 1)
                return null;

            return input[row, col];
        }
        
        public static TType GetFromArray<TType>(this TType[,] input, int row, int col, TType defaultValue)
        {
            if (row < 0 || row > input.GetLength(0) - 1)
                return defaultValue;

            if (col < 0 || col > input.GetLength(1) - 1)
                return defaultValue;

            return input[row, col];
        }

        public static int CountOccurences<TType>(this TType[,] input, TType needle)
        {
            var occurences = 0;
            for (var row = 0; row < input.GetLength(0); row++)
            for (var column = 0; column < input.GetLength(1); column++)
            {
                if (input[row, column].Equals(needle))
                    occurences++;
            }
                    

            return occurences;
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