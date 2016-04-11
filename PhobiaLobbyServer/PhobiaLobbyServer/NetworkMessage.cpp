#include "NetworkMessage.h"
#include "Utilities.h"

#define RECORD_SEPARATOR (char) 30
#define UNIT_SEPARATOR (char) 31

NetworkMessage::NetworkMessage(std::string unparsed)
{
	std::vector<std::string> records = split(unparsed, RECORD_SEPARATOR);
	for (int i = 0; i < records.size(); i++)
	{
		std::vector<std::string> units = split(records[i], UNIT_SEPARATOR);

		if (units.size() > 1)
		{
			elements[units[0]] = units[1];
		}
	}
}

bool NetworkMessage::hasKey(std::string key)
{
	return elements.count(key) > 0;
}