using System;
using System.Collections.Generic;
using System.Linq;

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
            ParsePackets(packetData);
            
            Part1();
            Part2();
        }
        
        private void Part1()
        {
            long versionBase = 0;
            versionBase += Packets.Select(GetVersionCount).Sum();
            
            Console.WriteLine($"Total versions: {versionBase}");
        }

        private void Part2()
        {
            long total = CalculatePacket(Packets.First());
            
            Console.WriteLine($"Total outer packet sum: {total}");
        }

        private void ParsePackets(string data)
        {
            while (data.Length > 0)
            {
                data = ParseFirstPacketAndReturnRest(data);
            }
        }

        private long GetVersionCount(Packet parent = null)
        {
            var version = parent.Version;

            version += parent.SubPackets.Select(GetVersionCount).Sum();

            return version;
        }

        private string ParseFirstPacketAndReturnRest(string data, int versionSum = 0, Packet? parentPacket = null)
        {
            if (data.Length < 6)
                return "";
            
            var version = data.Substring(0, 3).BinaryToDec();
            var type = data.Substring(3, 3).BinaryToDec();
            data = data.Substring(6);

            versionSum += Convert.ToInt32(version);
        
            switch (type)
            {
                case 4:
                    var packetData = "";
                    while (data.Substring(0, 1) != "0")
                    {
                        packetData += data.Substring(1, 4);
                        data = data.Substring(5);
                    }
                    packetData += data.Substring(1, 4);
                    data = data.Substring(5);

                    AddPacket(version, type, new List<long> { packetData.BinaryToDec() }, parentPacket);

                    return data;
                default:
                    var lengthTypeId = data.Substring(0, 1);
                    data = data.Substring(1);
                    
                    switch (lengthTypeId)
                    {
                        case "0":
                            var subPacketLength = data.Substring(0, 15).BinaryToDec();
                            data = data.Substring(15);

                            var subPacketString = data.Substring(0, Convert.ToInt32(subPacketLength));
                            data = data.Substring(Convert.ToInt32(subPacketLength));
                            
                            var newPacket = AddPacket(version, type, null, parentPacket);

                            while (!subPacketString.IsAllZeros())
                                subPacketString = ParseFirstPacketAndReturnRest(subPacketString, versionSum, newPacket);    
                            
                            return data;
                        case "1":
                            var numberOfSubPackets = data.Substring(0, 11).BinaryToDec();
                            data = data.Substring(11);

                            var newParentPacket = AddPacket(version, type, null, parentPacket);

                            for (var i = 0; i < numberOfSubPackets; i++)
                                data = ParseFirstPacketAndReturnRest(data, versionSum, newParentPacket);

                            return data;

                    }
                    break;
            }

            return data;
        }
        
        private Packet AddPacket(long version, long type, List<long>? literalValues = null, Packet? parent = null)
        {
            var newPacket = new Packet()
            {
                Version = version,
                TypeId = type,
                LiteralValues = literalValues ?? new List<long>(),
                SubPackets = new List<Packet>()
            };

            if (parent == null)
            {
                Packets.Add(newPacket);
                return newPacket;
            }
            
            parent.SubPackets.Add(newPacket);
            return newPacket;
        }
        
        private long CalculatePacket(Packet packet)
        {
            return packet.TypeId switch
            {
                0 => packet.SubPackets.Aggregate(0L, (sum, subPacket) => sum + CalculatePacket(subPacket)),
                1 => packet.SubPackets.Aggregate(1L, (product, subPacket) => product * CalculatePacket(subPacket)),
                2 => packet.SubPackets.Aggregate(Int64.MaxValue,
                        (min, subPacket) => Math.Min(min, CalculatePacket(subPacket))),
                3 => packet.SubPackets.Aggregate(Int64.MinValue,
                        (max, subPacket) => Math.Max(max, CalculatePacket(subPacket))),
                4 => packet.LiteralValues.Sum(),
                5 => CalculatePacket(packet.SubPackets.First()) > CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L,
                6 => CalculatePacket(packet.SubPackets.First()) < CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L,
                7 => CalculatePacket(packet.SubPackets.First()) == CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L,
                _ => int.MaxValue
            };
        }
    }
    
    public class Packet
    {
        public long Version { get; set; }
        public long TypeId { get; set; }
        public List<long> LiteralValues { get; set; } = new List<long>();
        public List<Packet> SubPackets { get; set; } = new List<Packet>();
    }

    public static class Helpers
    {
        public static bool IsAllZeros(this string tester)
        {
            return tester.IndexOf("1", StringComparison.Ordinal) == -1 || tester.Length == 0;
        }
        public static long BinaryToDec(this string binary)
        {
            return Convert.ToInt64(binary, 2);
        }

        public static string HexToBinary(this string hexValue)
        {
            return String.Join(String.Empty,
                hexValue.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );
        }
    }
    
}