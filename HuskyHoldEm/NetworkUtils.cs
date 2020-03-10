﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

/**
 * You must also update Client.GetUserInput() when updating this list
 */
public enum Command
{
	REGISTER_USER = 1,
	JOIN_GAME = 2,
	CREATE_GAME = 3,
	UNREGISTER_USER = 4,
	CLOSE_SOCKET = 5,
	VIEW_LEADERBOARD = 6,
	CHAT = 7,
	CHANGE_NAME = 8,
	GAME_RAISE = 9,
	GAME_STAY = 10,
	GAME_FOLD = 11,
	DISPLAY = 12
};

namespace HuskyHoldEm
{
	public static class NetworkUtils
	{
		public static Packet ReadPacket(Socket socket)
		{
			NetworkStream networkStream = new NetworkStream(socket);
			
			// message length buffer
			byte[] sizeBuffer = new byte[2];
			int bytes = networkStream.Read(sizeBuffer, 0, 2);
			ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
			Console.WriteLine("size = " + size);	// todo: debugging, remove before final submission
			
			// message buffer
			byte[] readBuffer = new byte[size];
			int bytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
			
			// clear for next packet
			networkStream.Flush();
			
			// deserialize back into a packet
			string jsonResponse = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
			return JsonConvert.DeserializeObject<Packet>(jsonResponse);
		}

		// TODO: might be good to have the "string input" be a Packet object
		// The packet object would have commands like "CHAT" "FOLD" "REGISTER" "JOIN" "CREATE" etc
		// The first line/word in the string could be the command to do
		// The rest of the string is the data for that command
		// Like a really simple HTTP
		public static void WritePacket(Socket socket, string message)
		{
			byte[] messageBuffer = ASCIIEncoding.ASCII.GetBytes(message);
			Console.WriteLine(message.Length);	// todo: debugging, remove before final submission
			
			// send message length
			byte[] sizeBuffer = new byte[2];
			ushort size = (ushort)message.Length;
			sizeBuffer[0] = (byte)size;
			sizeBuffer[1] = (byte)(size >> 8);
			socket.Send(sizeBuffer);
			
			// send message
			socket.Send(messageBuffer);
		}
	}

	/**
	 * Data packet sent between client and server.
	 */
	public class Packet
	{
		public Command Command { get; set; }
		public bool Success { get; set; }
		public List<object> DataList { get; set; }

		public Packet(Command command, bool success, List<object> dataList = null)
		{
			Command = command;
			Success = success;
			DataList = dataList;
		}
	}
}
