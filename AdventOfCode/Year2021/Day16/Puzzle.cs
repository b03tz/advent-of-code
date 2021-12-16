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

        private long CalculatePacket(Packet packet)
        {
            switch (packet.TypeId)
            {
                case 0:
                    // Sum
                    return packet.SubPackets.Aggregate(0L, (sum, subPacket) => sum + CalculatePacket(subPacket));
                case 1:
                    // Product
                    return packet.SubPackets.Aggregate(1L, (product, subPacket) => product * CalculatePacket(subPacket));                    
                case 2:
                    // Minimum
                    return packet.SubPackets.Aggregate(Int64.MaxValue, (min, subPacket) => Math.Min(min, CalculatePacket(subPacket)));
                case 3:
                    // Maximum
                    return packet.SubPackets.Aggregate(Int64.MinValue, (max, subPacket) => Math.Max(max, CalculatePacket(subPacket)));
                case 4:
                    // Literal
                    return packet.LiteralValues.Sum();
                case 5:
                    // Greater than
                    return CalculatePacket(packet.SubPackets.First()) > CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L;
                case 6:
                    // Less than
                    return CalculatePacket(packet.SubPackets.First()) < CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L;
                case 7:
                    // Equal to
                    return CalculatePacket(packet.SubPackets.First()) == CalculatePacket(packet.SubPackets.Last()) ? 1L : 0L;
            }

            return int.MaxValue;
        }

        private void ParsePackets(string data)
        {
            while (data.Length > 0)
            {
                data = ParseFirstPacketAndReturnRest(data);
            }
        }

        public long GetVersionCount(Packet parent = null)
        {
            var version = parent.Version;

            version += parent.SubPackets.Select(x => GetVersionCount(x)).Sum();

            return version;
        }

        public string ParseFirstPacketAndReturnRest(string data, int versionSum = 0, Packet? parentPacket = null)
        {
            if (data.Length < 6)
                return "";
            
            var version = data.Substring(0, 3).BinaryToDec();
            var type = data.Substring(3, 3).BinaryToDec();
            data = data.Substring(6);

            if (data.IsAllZeros())
                return "";

            versionSum += Convert.ToInt32(version);
        
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

                    if (parentPacket != null)
                    {
                        parentPacket.SubPackets.Add(new Packet
                        {
                            Version = version,
                            VersionSum = versionSum,
                            TypeId = type,
                            LiteralValues = new List<long> { packetData.BinaryToDec() }
                        });
                    }
                    else
                    {
                        Packets.Add(new Packet
                        {
                            Version = version,
                            VersionSum = versionSum,
                            TypeId = type,
                            LiteralValues = new List<long> { packetData.BinaryToDec() }
                        });
                    }

                    return data;
                default:
                    if (data.Length < 1)
                        return "";
                    
                    var lengthTypeId = data.Substring(0, 1);
                    data = data.Substring(1);
                    
                    switch (lengthTypeId)
                    {
                        case "0":
                            if (data.Length < 15)
                                return "";
                            
                            var subPacketLength = data.Substring(0, 15).BinaryToDec();
                            data = data.Substring(15);

                            var subPacketString = data.Substring(0, Convert.ToInt32(subPacketLength));
                            data = data.Substring(Convert.ToInt32(subPacketLength));

                            
                            var newPacket = new Packet()
                            {
                                Version = version,
                                TypeId = type,
                                SubPackets = new List<Packet>()
                            };
                            
                            if (parentPacket == null)
                                Packets.Add(newPacket);
                            else
                                parentPacket.SubPackets.Add(newPacket);

                            while (!subPacketString.IsAllZeros())
                                subPacketString = ParseFirstPacketAndReturnRest(subPacketString, versionSum, newPacket);    
                            
                            return data;
                        case "1":
                            if (data.Length < 11)
                                return "";
                            
                            var numberOfSubPackets = data.Substring(0, 11).BinaryToDec();
                            data = data.Substring(11);
                            
                            var newParentPacket = new Packet()
                            {
                                Version = version,
                                TypeId = type,
                                SubPackets = new List<Packet>()
                            };
                            
                            if (parentPacket == null)
                                Packets.Add(newParentPacket);
                            else
                                parentPacket.SubPackets.Add(newParentPacket);

                            for (var i = 0; i < numberOfSubPackets; i++)
                                data = ParseFirstPacketAndReturnRest(data, versionSum, newParentPacket);

                            return data;

                    }
                    break;
            }

            return data;
        }
    }

   
    
    public class Packet
    {
        public long Version { get; set; }
        public long VersionSum { get; set; }
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

        public static (string original, string substring) Cut(this string input, int length)
        {
            return (input.Substring(length), input.Substring(0, length));
        }
    }
    
}