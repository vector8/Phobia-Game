#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <winsock2.h>
#include <thread>
#include <string>
#include <iostream>
#include "NetworkMessage.h"
#include <unordered_map>

#pragma comment(lib, "ws2_32.lib")

#define BUFFLEN 4096
#define PORT 8889
#define HUMAN 0
#define MONSTER 1
#define RECORD_SEPARATOR (char) 30
#define UNIT_SEPARATOR (char) 31

std::thread recThread; SOCKET sock;
struct sockaddr_in sockAddr, senderAddr;
int slen;
bool running;

struct Client
{
	SOCKADDR_IN address;
	int playingAs;
};

struct Score
{
	std::string name;
	float score;
};

std::vector<Score> scores;

std::unordered_map<std::string, Client> clients;

void throwError(std::string msg)
{
	std::cout << msg << std::endl;
}

void cleanUp()
{
	running = false;
	recThread.join();
	shutdown(sock, SD_BOTH);
	closesocket(sock);
	WSACleanup();
}

void send(std::string message, SOCKADDR_IN &addr)
{
	if (sendto(sock, message.c_str(), message.length(), 0, (struct sockaddr *) &addr, sizeof(addr)) == SOCKET_ERROR)
	{
		throwError("sendto() failed with error code: " + WSAGetLastError());
	}
}

void receiveThread()
{
	char buffer[BUFFLEN];
	while (running)
	{
		memset(buffer, '\0', BUFFLEN);
		int recLenTemp = recvfrom(sock, buffer, BUFFLEN, 0, (struct sockaddr *) &senderAddr, &slen);

		int error = WSAGetLastError();
		if (error != WSAEWOULDBLOCK && error != 0)
		{
			std::string errorMsg = "recvfrom failed. Error code: " + std::to_string(error);
			throwError(errorMsg);
		}
		else if (error == 0)
		{
			// process received
			std::string received(buffer);
			NetworkMessage msg(received);

			std::string senderAddress(inet_ntoa(senderAddr.sin_addr));

			if (clients.count(senderAddress) == 0)
			{
				Client c;
				c.address = senderAddr;
				clients[senderAddress] = c;
			}

			if (msg.hasKey("T"))
			{
				std::string type = msg.elements["T"];
				if (type == "C")	// connect
				{
					std::cout << senderAddress << " connected." << std::endl;
					clients[senderAddress].playingAs = atoi(msg.elements["P"].c_str());

					std::string human, monster;
					bool matchFound = false;

					// loop through the client list to see if we can match people now
					for (auto& x : clients)
					{
						if (human.length() == 0 && x.second.playingAs == HUMAN)
						{
							human = x.first;
						}
						else if (monster.length() == 0 && x.second.playingAs == MONSTER)
						{
							monster = x.first;
						}

						if (human.length() != 0 && monster.length() != 0)
						{
							matchFound = true;
							break;
						}
					}

					if (matchFound)
					{
						std::cout << "Match found! Connecting " << human << " and " << monster << std::endl;

						Client humanClient = clients[human];
						Client monsterClient = clients[monster];

						// send them each a message
						std::string unitSeparator;
						unitSeparator += UNIT_SEPARATOR;
						std::string recordSeparator;
						recordSeparator += RECORD_SEPARATOR;
						std::string message = "T" + unitSeparator + "M" + recordSeparator + "A" + unitSeparator + monster;
						send(message, humanClient.address);
						send(message, monsterClient.address);

						// remove the clients from the queue
						clients.erase(human);
						clients.erase(monster);
					}
				}
				else if (type == "D")	// disconnect
				{
					// remove the client
					std::cout << senderAddress << " disconnected." << std::endl;
					clients.erase(senderAddress);
				}
				else if (type == "U")	// update high score list
				{
					std::cout << "Updating high scores with update from " << senderAddress << std::endl;
					float score = std::stof(msg.elements["S"]);

				}
				else if (type == "G")	// retrieve high score list
				{
					std::cout << "Sending high score list to " << senderAddress << std::endl;
				}
			}
		}
	}
}

int main()
{
	WSADATA wsa;

	//initialise Winsock
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		throwError("WSAStartup failed. Error Code: " + WSAGetLastError());
	}

	//create socket
	sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (sock == INVALID_SOCKET)
	{
		throwError("socket() failed with error code: " + WSAGetLastError());
	}

	//setup address structure
	sockAddr.sin_family = AF_INET;
	sockAddr.sin_port = htons(PORT);
	sockAddr.sin_addr.s_addr = INADDR_ANY;

	slen = sizeof(sockAddr);

	// If iMode!=0, non-blocking mode is enabled.
	u_long iMode = 1;
	ioctlsocket(sock, FIONBIO, &iMode);

	if (bind(sock, (struct sockaddr *)&sockAddr, sizeof(sockAddr)) == SOCKET_ERROR)
	{
		throwError("Bind failed with error code: " + WSAGetLastError());
	}

	running = true;
	recThread = std::thread(receiveThread);

	std::cout << "Server ready, waiting for connections..." << std::endl;

	std::string input;
	while (running)
	{
		std::getline(std::cin, input);
		if (tolower(input[0]) == 'q')
		{
			running = false;
			std::cout << "Shutting down..." << std::endl;
		}
	}

	cleanUp();
}