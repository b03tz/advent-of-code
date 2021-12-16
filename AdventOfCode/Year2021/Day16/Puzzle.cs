using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Year2021.Day16
{
    public class Puzzle : PuzzleBase
    {
        private List<Packet> Packets = new List<Packet>();
        public Puzzle()
        {
            this.Init(16, false);
            var line = this.GetPuzzleText();
            
            var packetData = line.HexToBinary();
            //ParsePackets("110100101111111000101000");
            //ParsePackets("11010010111111100010100000111000000000000110111101000101001010010001001000000000");
            //ParsePackets(packetData);
            ParsePackets("620080001611562C8802118E34".HexToBinary());
            //ParsePackets("11101110000000001101010000001100100000100011000001100000");
        }

        private void ParsePackets(string data)
        {
            while (data.Length > 0)
            {
                data = ParseFirstPacketAndReturnRest(data);
            }
        }

        public string ParseFirstPacketAndReturnRest(string data, int versionSum = 0, Packet parentPacket = null)
        {
            if (data.Length < 6)
                return "";
            
            var version = data.Substring(0, 3).BinaryToDec();
            var type = data.Substring(3, 3).BinaryToDec();
            data = data.Substring(6);

            versionSum += version;
        
            switch (type)
            {
                case 4:
                    // It's a literal value keep reading in groups of 5 until we find a 0 bit
                    var packetData = "";
                    while (data.Substring(0, 1) != "0")
                    {
                        packetData += data.Substring(1, 4);
                        data = data.Substring(5);
                    }
                    packetData += data.Substring(1, 4);
                    data = data.Substring(5);

                    Packets.Add(new Packet
                    {
                        Version = version,
                        VersionSum = versionSum,
                        TypeId = type,
                        LiteralValues = new List<int> { packetData.BinaryToDec() }
                    });
                    
                    // Keep removing 0's
                    /*while (data.Length > 0 && data.Substring(0, 1) == "0")
                        data = data.Substring(1);*/

                    return data;
                default:
                    var lengthTypeId = data.Substring(0, 1);
                    data = data.Substring(1);

                    switch (lengthTypeId)
                    {
                        case "0":
                            var subPacketLength = data.Substring(0, 15).BinaryToDec();
                            data = data.Substring(15);

                            var subPacketString = data.Substring(0, subPacketLength);
                            data = data.Substring(subPacketLength);

                            var subPackets = new List<string>();
                            
                            // Unpack subpackets
                            while (subPacketString.Length > 5)
                            {
                                var subPacketHeader = subPacketString.Substring(0, 6);
                                var subPacketData = subPacketString.Substring(6);
                                subPacketString = subPacketString.Substring(6);
                                
                                // Keep reading subpacket data in blocks of 5 until 0
                                var readSubPacket = "";
                                while (subPacketData.Substring(0, 1) != "0")
                                {
                                    readSubPacket += subPacketData.Substring(0, 5);
                                    subPacketData = subPacketData.Substring(5);
                                    subPacketString = subPacketString.Substring(5);
                                }
                                
                                readSubPacket += subPacketData.Substring(0, 5);
                                subPacketData = subPacketData.Substring(5);
                                subPacketString = subPacketString.Substring(5);
                                
                                subPackets.Add(subPacketHeader + readSubPacket);
                            }

                            foreach (var subPacket in subPackets)
                            {
                                ParseFirstPacketAndReturnRest(subPacket, versionSum);
                            }

                            return data;
                        case "1":
                            var numberOfSubPackets = data.Substring(0, 11).BinaryToDec();
                            data = data.Substring(11);

                            for (var i = 0; i < numberOfSubPackets; i++)
                            {
                                data = ParseFirstPacketAndReturnRest(data, versionSum);
                            }

                            return data;

                    }
                    break;
            }

            return data;
        }
    }

   
    
    public class Packet
    {
        public int Version { get; set; }
        public int VersionSum { get; set; }
        public int TypeId { get; set; }
        public List<int> LiteralValues { get; set; } = new List<int>();
        public List<Packet> SubPackets { get; set; }
    }

    public static class Helpers
    {
        public static int BinaryToDec(this string binary)
        {
            return Convert.ToInt32(binary, 2);
        }

        public static string HexToBinary(this string hexValue)
        {
            return String.Join(String.Empty,
                hexValue.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );
        }

        public static (string original, string substring) Cut(this string input, int length)
        {
            return (input.Substring(length), input.Substring(0, length));
        }
    }
    
}