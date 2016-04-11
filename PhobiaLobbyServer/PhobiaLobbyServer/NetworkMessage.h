#pragma once
#include <string>
#include <unordered_map>

class NetworkMessage
{
public:
	NetworkMessage(std::string unparsed);

	std::unordered_map<std::string, std::string> elements;

	bool hasKey(std::string key);
};