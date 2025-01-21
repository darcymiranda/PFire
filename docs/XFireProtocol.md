<style>
h1 {
  text-align: center;
  color: #005A9C;
}

h2 {
  color: #005A9C;
  
  /* Add 1 to chapter */
  counter-increment: chapter;

  /* Set section to 0 */
  counter-reset: section 0;
}

h2:before {
  content: counter(chapter) ". ";
}

h3 {
  color: #005A9C;
  counter-increment: section;
}

h3:before {
  content: counter(chapter) "." counter(section) " ";
}

table {
  border: 1px solid black;
}

th {
  background: #ccc;
}
</style>

# XFire Protocol Specification

# Iain McGinniss, 2007/08/16

* * *

This document aims to provide a full specification for the XFire instant messaging protocol. A lot of the information found in this document would not be available if it were not for the hard work of the xfirelib project team (http://xfirelib.sphene.net), who reverse engineered the protocol to write xfirelib. This information, along with some of my own findings from writing the [OpenFire](http://code.google.com/p/openfire) library and suite of tools, provide the basis for this document which aims to formally document what we discovered, and to be provided as a basis for implementing XFire communication libraries in alternative languages.

## High Level Details

The XFire protocol uses a mix of client/server communication and peer to peer communication. Client/server communication is carried out over a TCP/IP connection, with the client connecting to the XFire central server (cs.xfire.com) on port 25999. Peer to peer communication is performed using UDP datagrams, with the ports involved negotiated between the two peers.

As a rule, XFire uses little endian byte ordering and UTF-8 encoding for strings.

### Message Structure

The messages between client and server follow a common pattern:

|    0,1   |     2,3     |     4     |      5...     |
| :------: | :---------: | :-------: | :-----------: |
| MSG SIZE | MSG TYPE ID | NUM ATTRS | ATTRIBUTES... |

1.  The message size, as a 16-bit integer.
2.  The message type id. Each message type, from a login request to a chat message, has a unique 16-bit integer ID.
3.  The number of attributes within the message, as an 8-bit integer.
4.  The list of attributes for the message with their associated values.

Message attributes are formatted as follows:

|         0         |      1..n      |       n+1       |      n+2...     |
| :---------------: | :------------: | :-------------: | :-------------: |
| ATTR NAME LEN (n) | ATTRIBUTE NAME | ATTR VALUE TYPE | ATTR VALUE DATA |

1.  The attribute name length, as an 8-bit integer.
2.  The attribute name, as an ISO/IEC 8859-1 encoded string (i.e. 8 bits per character).
3.  The attribute value.

The attribute value can be one of a set of predefined attribute types:

| Type ID | Description                                                                                                                                                                                              |
| :------ | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 0x01    | A variable length string. The first two bytes of the data containa 16-bit integer indicating the length of the string, followed by the string data.                                                      |
| 0x02    | A 32-bit int.                                                                                                                                                                                            |
| 0x03    | A 128-bit session identifier, used to identify a particular user'ssession on the xfire network.                                                                                                          |
| 0x04    | A list. The first byte indicates the type of the items in the list,for instance 0x01 for a string list. The next two bytes indicate the number of items in the list, followed by each consecutive value. |
| 0x05    | A string keyed map. The first byte indicates the number of entries,followed by each entry, with a string name (prefixed by 8-bit string length) and type (the same format for messages as a whole).      |
| 0x06    | A DID value. As yet, the purpose of this value is unknown (See [DID Message](#did-message) for more information).                                                                                        |
| 0x09    | An integer keyed map (type 0x09). The first byte indicates the number of entries, followed by each entry, with an 8-bit integer key and type.                                                            |

An example of a simple message is the [client version message](#client-version-message), which has a single attribute named `version`. It looks like this (with each bytes value displayed in hexadecimal, or the character equivalent where appropriate):

| Position (hex)  |  00 |  01 |  02 |  03 |  04 |  05 |  06 |  07 |  08 |  09 |  0A |  0B |  0C |  0D |  0E |  0F |  10 |  11 |
| :-------------- | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Value (hex)     |  12 |  00 |  03 |  00 |  01 |  07 |  76 |  65 |  72 |  73 |  69 |  6f |  6e |  02 |  43 |  00 |  00 |  00 |
| Value (literal) |     |     |     |     |     |     |  v  |  e  |  r  |  s  |  i  |  o  |  n  |     |     |     |     |     |

As you can see from the above, the message length is 18 (0x0012), the packet id is 3 (0x0003), there is a single attribute with name of length 7 (`version`), with a 32-bit value 0x00000043 (67 represented as a 32-bit int). Remember that because the protocol is little endian, the integers will appear with the least significant byte first, i.e. the message length will appear as 12 00 in the message.

### Connection Handshake

When initiating a new connection, the sequence of events is as follows:

1. The client opens a TCP/IP connection with the server, first sending the hexadecimal code 55 41 30 31 (or, as a string, `UA01`) as an opening statement.
2. The client then sends a [client information message](#client-information-message), followed by a [client version message](#client-version-message).
    1. If this version is older than the current XFire version, a [new version message](#new-version-available-message) will be sent and the server will send no more messages. The client should disconnect at this point.
3. The server will then send an [authentication challenge message](#login-challenge-message) to the client.
4. In response to this, the client will send a [login request message](#login-request-message), containing the user name and a SHA-1 hashed version of the password, combined with a salt string and constant for extra security.
5. The server will either respond with a [login success](#login-success-message) or [failure message](#login-failure-message).

Once a client has successfully logged in, the server will send a flurry of messages used to initialise the client (who the [user's friends are](#friend-list-message), [who is online](#session-id-list-message), and so on). Other messages follow a request / response model, where the client must send a message which triggers the server to send a matching response message (for instance, checking to see what friends of friends are online).

## Message Types

| Message ID                                              | Description                             | Key type | To Server | From Server | From Peer (UDP) |
| :------------------------------------------------------ | :-------------------------------------- | :------: | :-------: | :---------: | :-------------: |
| [1](#login-request-message)                             | Login request                           | `string` |     ✔     |      ❌      |        ❌        |
| [2](#chat-message)                                      | Chat message                            | `string` |     ✔     |      ❌      |        ✔        |
| [3](#client-version-message)                            | Client version                          | `string` |     ✔     |      ❌      |        ❌        |
| [5](#friends-of-online-friend-request-message)          | Friends of online friend request        | `string` |     ✔     |      ❌      |        ❌        |
| [6](#outgoing-friend-invitation-message)                | Outgoing friend invitation              | `string` |     ✔     |      ❌      |        ❌        |
| [7](#accept-invitation-message)                         | Accept Invitation                       | `string` |     ✔     |      ❌      |        ❌        |
| [8](#reject-invitation-message)                         | Reject Invitation                       | `string` |     ✔     |      ❌      |        ❌        |
| [12](#user-lookup-message)                              | User lookup                             | `string` |     ✔     |      ❌      |        ❌        |
| [13](#connection-keep-alive-message)                    | Connection keepalive                    | `string` |     ✔     |      ❌      |        ❌        |
| [16](#client-configuration-message)                     | Client configuration                    | `string` |     ✔     |      ❌      |        ❌        |
| 17                                                      | Connection information                  | `string` |     ✔     |      ❌      |        ❌        |
| [18](#)                                                 | Client information                      | `string` |     ✔     |      ❌      |        ❌        |
| 23                                                      | ???                                     | `string` |     ✔     |      ❌      |        ❌        |
| 24                                                      | ???                                     | `string` |     ✔     |      ❌      |        ❌        |
| 25                                                      | ???                                     | `string` |     ✔     |      ❌      |        ❌        |
| [26](#group-create-message)                             | Group Create                            | `string` |     ✔     |      ❌      |        ❌        |
| [128](#login-challenge-message)                         | Login challenge                         | `string` |     ❌     |      ✔      |        ❌        |
| [129](#login-failure-message)                           | Login failure                           | `string` |     ❌     |      ✔      |        ❌        |
| [130](#login-success-message)                           | Login success                           | `string` |     ❌     |      ✔      |        ❌        |
| [131](#friend-list-message)                             | Friend list                             | `string` |     ❌     |      ✔      |        ❌        |
| [132](#session-id-list-message)                         | Session ID list                         | `string` |     ❌     |      ✔      |        ❌        |
| [133](#server-routed-chat-message)                      | Server routed chat message              | `string` |     ❌     |      ✔      |        ❌        |
| [134](#new-version-available-message)                   | New version available                   | `string` |     ❌     |      ✔      |        ❌        |
| [135](#friend-game-information-message)                 | Friend game information                 | `string` |     ❌     |      ✔      |        ❌        |
| [136](#friends-of-friends-message)                      | Friends of friends                      | `string` |     ❌     |      ✔      |        ❌        |
| [137](#outgoing-friend-invitation-confirmation-message) | Outgoing friend invitation confirmation | `string` |     ❌     |      ✔      |        ❌        |
| [138](#incoming-friend-invitation-message)              | Incoming friend invitation              | `string` |     ❌     |      ✔      |        ❌        |
| 141                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| [143](#user-search-results-message)                     | User search results                     | `string` |     ❌     |      ✔      |        ❌        |
| [147](#friend-voip-information-message)                 | Friend VoIP information                 | `string` |     ❌     |      ✔      |        ❌        |
| 148                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| 151                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| 152                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| [153](#group-create-confirmation-message)               | Group Create Confirmation               | `string` |     ❌     |      ✔      |        ❌        |
| [154](#friend-status-message)                           | Friend status                           | `string` |     ❌     |      ✔      |        ❌        |
| 155                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| [156](#extra-friend-game-information-message)           | Extra friend game information           | `string` |     ❌     |      ✔      |        ❌        |
| 157                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| 163                                                     | ???                                     |   `8-bit int`   |     ❌     |      ✔      |        ❌        |
| [400](#did-message)                                     | DID (?)                                 | `string` |     ❌     |      ✔      |        ❌        |
| [450](#channel-information-message)                     | Channel information (?)                 |   `8-bit int`   |     ❌     |      ✔      |        ❌        |

### Login Request Message

This message is sent by the client to the server in response to a [Login Challenge Message](#login-challenge-message) in order to authenticate the user. The server will respond with either a [Login Success Message](#login-success-message) or a [Login Failure Message](#login-failure-message).

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 1          |      `string`      | Client to Server |

#### Contents

| Attribute Name |                      Type                     | Details                                                                                                                                                                                                                                                                                                                                                                                           |
| :------------- | :-------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| name           |                    `string`                   | The user name of the user that is attempting to log in.                                                                                                                                                                                                                                                                                                                                           |
| password       | `string` (128 bit SHA-1 hash as a hex string) | The salted username and password of the user that is attempting to log in. This is constructed as follows:<pre>String a = username + password + "UltimateArena";<br />int128 a_hash = sha1(a);<br />String b = toHexString(a_hash) + salt;<br />int128 b_hash = sha1(b);<br />String result = toHexString(b_hash);</pre>Where the salt is a string that has been provided in the login challenge. |
| flags          |                  `32-bit int`                 | The purpose of this value is not known, in all data observed from the real XFire client it has been 0.                                                                                                                                                                                                                                                                                            |

### Chat Message

This message is sent by the client to the server, or directly to another user via UDP. The message can represent a number of things:

* An content message (e.g. `hi there`)
* An indication of whether the user is typing or not
* A list of the client's details (IP address, open UDP port, etc)
* An acknowledgement that a message from the peer was received

When chat messages are sent via the server, they will be received by the peer as a [Server Routed Chat Message](#server-routed-chat-message). 

If communication is to be routed via the XFire server, client information messages should be sent regularly to prevent high-latency warnings where the peer is a real XFire client. In OpenFire, these are sent with each real message, up to once every 5 seconds. It has not been determined what the threshold is for the XFire client to start reporting latency warnings.

#### Properties

| Message ID | Attribute Key Type |              Direction             |
| :--------- | :----------------: | :--------------------------------: |
| 2          |      `string`      | Client to Server, Client to Client |

#### Contents

| Attribute Name |       Type       | Details                                                                                |
| :------------- | :--------------: | :------------------------------------------------------------------------------------- |
| sid            |    Session ID    | The Session ID of the peer this message is intended for.                               |
| peermsg        | string keyed map | Contains the real content of the message. See below for the structure of each payload. |

####  Peermsg contents

The contents of the peermsg map vary depending on what this chat message represents. What is represented is indicated in the `msgtype` attribute in the map, a `32-bit int` value.

* _msgtype = 0_ - a content message:

| Attribute Name |     Type     | Details                                                                                                                      |
| :------------- | :----------: | :--------------------------------------------------------------------------------------------------------------------------- |
| imindex        | `32-bit int` | The index of this message within the conversation log. Used as a hint of message order, incremented for each message sent. |
| im             | `string` | The actual message (e.g. `hello`)                                                                                            |

* _msgtype = 1_ - an acknowledgement message:

| Attribute Name | Type           | Details                                                                                                   |
|:----|:---:|:----|
| imindex        | `32-bit int` | The index of the message we are acknowledging. Corresponds to the `imindex` in the message the peer sent. |

* _msgtype = 2_ - a client information message:

| Attribute Name |               Type               | Details                                                                                                                                      |
| :------------- | :------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------- |
| ip             |     IPv4 address (32-bit int)    | The public IPv4 address of this user.                                                                                                        |
| port           | 32-bit int (legal range 0-65535) | The public UDP port of this user.                                                                                                            |
| localip        |     IPv4 address (32-bit int)    | The LAN IPv4 address of this user.                                                                                                           |
| localport      | 32-bit int (legal range 0-65535) | The LAN UDP port of this user (not necessarily the same as the public port, depending on how the use of NAT or firewall)                     |
| status         |           `32-bit int`           | The purpose of this field has not been determined, in all analysed data to date it has been 0.                                               |
| salt           |             `string`             | A salt string; it is not known what this is used for (it is always present but does not appear to be used when communicating via the server) |

* _msgtype = 3_ - a typing notification:

| Attribute Name |     Type     | Details                                                                                    |
| :------------- | :----------: | :----------------------------------------------------------------------------------------- |
| imindex        | `32-bit int` | The index of the message that will be sent if the user sends the pending content message.  |
| typing         | `32-bit int` | A boolean indicating whether the user is typing or not (`1` is typing, `0` is not typing). |

### Client Version Message

This message is sent by the client to the server immediately after a [client information message](#client-information-message) in order to report the version of the xfire client in use. This number changes frequently, incremented with each release of the XFire client (typically at least once a month). If the version number sent to the server is lower than the current released client, the server will send a [new version available](#new-version-available-message) message and cease communicating with the client (the TCP connection will remain open until the client closes it, but it will ignore any further messages from the client).

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 3          |      `string`      | Client to Server |

#### Contents

| Attribute Name |     Type     | Details                           |
| :------------- | :----------: | :-------------------------------- |
| version        | `32-bit int` | The version number of the client. |

### Friends of Online Friend Request Message

This message is sent by the client to the server to get a list of friends of friends who are online and currently playing a game. The client sends a list of session IDs of friends who are currently online, and the server will respond with a [friends of friends](#friends-of-friends-message) message. 

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 5          |      `string`      | Client to Server |

#### Contents

| Attribute Name |       Type      | Details                                                                                                                  |
| :------------- | :-------------: | :----------------------------------------------------------------------------------------------------------------------- |
| sid            | Session ID list | A list of session IDs of currently online friends, for whom we wish to acquire the list of second-degree online friends. |

### Outgoing Friend Invitation Message

This is the message sent by a client when it wishes to add another user to it's friend list. It contains the username of the user to invite and a personalised message which will be displayed to the other user.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 6          |      `string`      | Client to Server |

#### Contents

| Attribute Name |  Type  | Details                                                                                              |
| :------------- | :----: | :--------------------------------------------------------------------------------------------------- |
| name           | `string` | The username of the user to invite.                                                                  |
| msg            | `string` | The personalised message typed by the user of the client, intended to be displayed to the recipient. |

### Accept Invitation Message

This is the message sent by a client when it wishes to accept an [invitation](#incoming-friend-invitation-message) received from another user. It simply contains the username of the other user.

This message type is identical to the [reject invitation message](#reject-invitation-message), the message ID is the only difference.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 7          |      `string`      | Client to Server |

#### Contents

| Attribute Name |  Type  | Details                                                 |
| :------------- | :----: | :------------------------------------------------------ |
| name           | `string` | The username of the user to accept the invitation from. |

### Reject Invitation Message

This is the message sent by a client when it wishes to reject an [invitation](#incoming-friend-invitation-message) received from another user. It simply contains the username of the other user.

This message type is identical to the [accept invitation message](#accept-invitation-message), the message ID is the only difference.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 8          |      `string`      | Client to Server |

#### Contents

| Attribute Name |  Type  | Details                                                 |
| :------------- | :----: | :------------------------------------------------------ |
| name           | `string` | The username of the user to reject the invitation from. |

### User Lookup Message

This is the message sent by a client when a search for friends is executed. Searches can be conducted using first name, last name, email address, or any combination thereof.

The server will respond to this message with a [user search results message](#user-search-results-message).

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 12         |      `string`      | Client to Server |

#### Contents

| Attribute Name |   Type   | Details                                                                                |
| :------------- | :------: | :------------------------------------------------------------------------------------- |
| name           | `string` | A search string, which will match against email addresses, first names and last names. |
| fname          | `string` | A search string which will only match the first name of other users.                   |
| lname          | `string` | A search string which will only match the last name of other users.                    |
| email          | `string` | A search string which will only match the email addresses of other users.              |

### Connection Keep-alive Message

This message is sent periodically by the client to notify the server it is still alive. It is not known how long a client can wait before sending a keepalive message, however the OpenFire library sends one every 60 seconds if there is no other outgoing connection activity.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 13         |      `string`      | Client to Server |

#### Contents

| Attribute Name |        Type       | Details                                                                                       |
| :------------- | :---------------: | :-------------------------------------------------------------------------------------------- |
| value          |    `32-bit int`   | An integer of unknown purpose. In all analysed data to date this has had the value `0`.       |
| stats          | `32-bit int` list | An integer list of unknown purpose. In all analysed data to date this has been an empty list. |

### Client Configuration Message

This is a message sent by the client immediately after a [successful login](#login-success-message), and describes some of the user's chosen configuration (skin, theme, etc) to the XFire server.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 16         |      `string`      | Client to Server |

#### Contents

| Attribute Name |   Type   | Details                                                                                                           |
| :------------- | :------: | :---------------------------------------------------------------------------------------------------------------- |
| lang           | `string` | The locale string of the user, e.g. `en` for english or `de` for german.                                          |
| skin           | `string` | The skin currently being used by the user's client. In a vanilla XFire client, this will typically be `XFire`.    |
| theme          | `string` | The theme currently being used by the user's client. In a vanilla XFire client, this will typically be `default`. |
| partner        | `string` | A string of unknown purpose. In all data analysed to date, this has been an empty string.                         |

### Client Information Message

This message is sent by the client to the server as the first message after the [initial handshake](#connection-handshake). It contains a version number and the list of skins installed by the client.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 18         |      `string`      | Client to Server |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                                                                                                                                                                              |
| :------------- | :---------------: | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| version        | `32-bit int` list | A version number, in 4 parts. As of writing, all observed messages have used the version number `3.2.0.0`. It appears this may have been made redundant by the version number transmitted in the client version message.                                                                                                             |
| skin           |   `string` list   | The list of skins installed on the client. With a vanilla install of the XFire client, this contains the strings `Xfire`, `standard`, `Separator`, and `XF_URL`. This corresponds to the XFire client's `Tools -> Skin` menu, with string representations of the horizontal separator and the link to the XFire skins download page. |

### Group Create Message

This message is sent by the client to the server when creating a group, passing the name of the group.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 26         |      `string`      | Client to Server |

#### Contents

| Attribute Name |   Type   | Details               |
| :------------- | :------: | :-------------------- |
| 0x1A           | `string` | The name of the group |


### Login Challenge Message

This message is sent by the server to the client to initiate the authentication process as part of the [initial handshake](#connection-handshake). It contains the salt that is to be used to obscure the user's password, preventing replay attacks.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 128        |      `string`      | Server to Client |

#### Contents

| Attribute Name |   Type   | Details                                                         |
| :------------- | :------: | :-------------------------------------------------------------- |
| salt           | `string` | A salt string which is to be used as part of the login request. |

### Login Failure Message

This message is sent in response to a [login request message](#login-request-message) if the user's password was incorrect.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 129        |      `string`      | Server to Client |

#### Contents

| Attribute Name |     Type     | Details                                                                                                           |
| :------------- | :----------: | :---------------------------------------------------------------------------------------------------------------- |
| reason         | `32-bit int` | A code perhaps used to indicate the reason for the failed login. In all analysed data this has had the value `0`. |

### Login Success Message

This message is sent in response to a [login request message](#login-request-message) if the user authenticated successfully. It contains a variety of information that is used to initialise the client.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 130        |      `string`      | Server to Client |

#### Contents

| Attribute Name |     Type     | Details                                                                                                                                                                                                                                                                                 |
| :------------- | :----------: | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| userid         | `32-bit int` | The user's unique identifier on the network, bound to their username. This is used in various other parts of the protocol instead of the username.                                                                                                                                      |
| sid            |  Session ID  | The unique Session ID for this connection, distinct from all other connections this user or any other user's have had.                                                                                                                                                                  |
| nick           |   `string`   | The nickname or alias of this user last time they connected - this is what is displayed in the XFire UI if one has been set, otherwise the username is displayed.                                                                                                                     |
| status         | `32-bit int` | An integer of unknown purpose.                                                                                                                                                                                                                                                          |
| dlSet          |   `string`   | A string of unknown purpose. In all observed data this has been an empty string.                                                                                                                                                                                                        |
| p2pset         |   `string`   | A string of unknown purpose. In all observed data this has been an empty string.                                                                                                                                                                                                        |
| clntSet        |   `string`   | A string of unknown purpose. In all observed data this has been an empty string.                                                                                                                                                                                                        |
| minRect        | `32-bit int` | An integer of unknown purpose, presumably related in some way to the rectangular size of the client. In all observed data this has had the value `1`.                                                                                                                                   |
| maxRect        | `32-bit int` | An integer of unknown purpose, presumably related in some way to the rectangular size of the client. In all observed data this has had the value `1800`, and the XFire client does seem restricted to roughly this size (it won't fill the width of a 1920x1200 monitor, for instance). |
| ctry           | `32-bit int` | An integer of unknown purpose.                                                                                                                                                                                                                                                          |
| n1             | IPv4 address | An address of unknown purpose. In all observed data this has had the value `204.71.190.131`, which resolves to `nat1.sv.xfire.com`.                                                                                                                                                     |
| n2             | IPv4 address | An address of unknown purpose. In all observed data this has had the value `204.71.190.132`, which resolves to `nat2.sv.xfire.com`.                                                                                                                                                     |
| n3             | IPv4 address | An address of unknown purpose. In all observed data this has had the value `204.71.190.133`, which resolves to `nat3.sv.xfire.com`.                                                                                                                                                     |
| pip            | IPv4 address | The public IPv4 address of this client, as detected by the server. This is useful for the client to discover it's public IP when it is not directly connected to the internet.                                                                                                          |

### Friend List Message

This message provides the list of friends for the current user after a successful login, with some essential details for future operations dealing with these friends.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 131        |      `string`      | Server to Client |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                            |
| :------------- | :---------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| userid         | `32-bit int` list | A list containing the unique ID of each friend.                                                                                                                                    |
| friends        |   `string` list   | A list containing the usernames of each friend.                                                                                                                                    |
| nick           |   `string` list   | A list containing the nickname of each friend, i.e. the name that will be displayed instead of the username if set. If no nickname has been set, this will be an empty string. |

### Session ID List Message

This message provides a list of session IDs for friends who are currently online, or for friends who have just come online mid-way through a session. This message type is also used to indicate when friends have gone offline.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 132        |      `string`      | Server to Client |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                                        |
| :------------- | :---------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| userid         | `32-bit int` list | A list containing the unique ID of each who has come online or gone offline.                                                                                                                   |
| sid            |  Session ID list  | A list containing the session IDs for each user. If the Session ID is 0, this indicates that the friend has gone offline. Otherwise, it is a real Session ID and indicates the user is online. |

### Server Routed Chat Message

This message type is identical to a normal [chat message](#chat-message), but is sent down to the client via the TCP link to the server. This allows clients to communicate where they cannot open a public UDP port and directly communicate with peers.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 133        |      `string`      | Server to Client |

#### Contents

See the description of the normal [chat message](#chat-message) for a full description of the contents of the message.

### New Version Available Message

This message is sent by the server to the client if it reports a version number which is lower than the currently released XFire client. It contains the details to download newer client versions. The message is composed of a number of string lists, where values at matching indices in these lists collectively represent a new version.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 134        |      `string`      | Server to Client |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                                      |
| :------------- | :---------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| version        | `32-bit int` list | The list of new version numbers available.                                                                                                                                                   |
| file           |   `string` list   | The links to the new XFire client versions. An example of a URL sent is . All files observed to date have been .exe update installers which  must be run within the XFire installation path. |
| command        | `32-bit int` list | An integer list of unknown purpose. In all data observed so far, the integers have had the value `1`.                                                                                        |
| fileid         | `32-bit int` list | An integer list of unknown purpose. In all data observed so far, the integers have had the value `0`.                                                                                        |
| flags          |    `32-bit int`   | An integer of unknown purpose. In all data observed so far this has had the value `0`.                                                                                                       |

### Friend Game Information Message

This message contains a description of games that friends are currently playing.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 135        |      `string`      | Server to Client |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                       |
| :------------- | :---------------: | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| sid            |  Session ID list  | A list containing the session IDs for each user that is playing a game.                                                                                                       |
| gameid         | `32-bit int` list | A list containing the game ids currently being played by each user. These correspond to the game IDs contained in the xfire_games.ini                                         |
| gip            | IPv4 address list | A list containing the IP addresses of the servers for each game being played by each user. If the game has no server IP or it cannot be determined, then the value will be 0. |
| gport          | `32-bit int` list | A list containing the port of the servers for each game being played by each user. If the port cannot be determined, then the value will be 0.                                |

### Friends of Friends Message

This message contains a list of users who are friends of friends and are currently playing a game. This is sent in response to a [friends of online friend request message](#friends-of-online-friend-request-message).

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 136        |      `string`      | Server to Client |

#### Contents

| Attribute Name |            Type            | Details                                                     |
| :------------- | :------------------------: | :---------------------------------------------------------- |
| fnsid          |       Session ID list      | A list containing the Session ID of a friend of a friend.   |
| userid         |      `32-bit int` list     | A list containing the user ID of a friend of a friend.      |
| uname          |        `string` list       | A list containing the user name of a friend of a friend.    |
| nick           |        `string` list       | A list containing the display name of a friend of a friend. |
| friends        | `32-bit int` list of lists | A list containing the friends of the friend of a friend.    |

### Outgoing friend invitation confirmation message

> TODO: describe this message type.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 137        |      `string`      | Server to Client |

#### Contents

| Attribute Name | Type | Details |
| :------------- | :--: | :------ |
| TBD            |  TBD | TBD     |

### Incoming Friend Invitation Message

Incoming friend invitation messages are sent to the client by the server when a peer is requesting to add them to their friends list.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 138        |      `string`      | Server to Client |

#### Contents

| Attribute Name |      Type     | Details                                                                                   |
| :------------- | :-----------: | :---------------------------------------------------------------------------------------- |
| name           | `string` list | A list containing the user names of all the peers requesting to become a friend.          |
| nick           | `string` list | A list containing the display names of all the peers requesting to become a friend.       |
| msg            | `string` list | A list containing the invitation messages from each user (e.g. `will you be my friend?`). |

### User Search Results Message

This message is sent by the server to the client in response to a [user lookup message](#user-lookup-message). It contains the details (user name, first name, last name and email address) of all users that matched the provided query.

The message contains a set of string lists, where strings at corresponding indices in the lists form the information for a matching user.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 143        |      `string`      | Server to Client |

#### Contents

| Attribute Name |      Type     | Details                                         |
| :------------- | :-----------: | :---------------------------------------------- |
| name           | `string` list | The list of user names for matching users.      |
| fname          | `string` list | The list of first names for matching users.     |
| lname          | `string` list | The list of last names for matching users.      |
| email          | `string` list | The list of email addresses for matching users. |

### Friend VoIP Information message

This message is sent to the client by the server to notify them of any VoIP servers in use by their friends.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 147        |      `string`      | Server to Client |

#### Contents

| Attribute Name |        Type       | Details                                                                                                                                                                   |
| :------------- | :---------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| sid            |  Session ID list  | The list of session ids for which VoIP information has been updated.                                                                                                      |
| vid            | `32-bit int` list | The IDs of the VoIP applications that are being used for each user. These relate to the IDs stored in the xfire_games.ini file that comes with the standard XFire client. |
| vip            | IPv4 address list | The list of IP addresses of the VoIP servers for each user.                                                                                                               |

### Group Create Confirmation Message

This message is sent to the client by the server to notify it that the [create group message](#create-group-message) was successful.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 153        |      `string`      | Server to Client |

#### Contents

| Attribute Name |     Type     | Details               |
| :------------- | :----------: | :-------------------- |
| 0x19           | `32-bit int` | The id of the group   |
| 0x1A           |   `string`   | The name of the group |

### Friend Status Message

This message is sent to the client by the server to notify it of a change in a friend's status message (e.g. `AFK` or `out to lunch`).

The message contains a set of string lists, where strings at corresponding indices in the lists form each individual status update.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 154        |      `string`      | Server to Client |

#### Contents

| Attribute Name |       Type      | Details                                                            |
| :------------- | :-------------: | :----------------------------------------------------------------- |
| sid            | Session ID list | The list of session ids of users whose status message has changed. |
| msg            |  `string` list  | The list new status messages;                                      |

### Extra Friend Game Information Message

> TODO: describe this message type.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 156        |      `string`      | Server to Client |

#### Contents

| Attribute Name | Type | Details |
| :------------- | :--: | :------ |
| TBD            |  TBD | TBD     |

### DID Message

The purpose of this message type is unknown. Unusually, it also contains an entirely unique data-type, consisting of a 21 byte value. It is sent from the server to the client at the start of a session, and does not appear to be used thereafter.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 400        |      `string`      | Server to Client |

#### Contents

| Attribute Name |    Type   | Details                    |
| :------------- | :-------: | :------------------------- |
| did            | DID value | The mystery 21-byte value. |

### Channel Information Message

> TODO: describe this message type.

#### Properties

| Message ID | Attribute Key Type |     Direction    |
| :--------- | :----------------: | :--------------: |
| 450        |     `8-bit int`    | Server to Client |

#### Contents

| Attribute Name | Type | Details |
| :------------- | :--: | :------ |
| TBD            |  TBD | TBD     |