**ONLY FOR 64 BIT WINDOWS PRO BUILDS!**

**WORKS AS OF 13/10/2025 UPDATES TO THE GAME MIGHT BREAK IT**

Pokemon revolution online bot that searches for rare pokemon automatically

Uses multi level pointers under the hood

## Guide to adding discord bot functionality

1. Go [here](https://discord.com/developers/applications?new_application=true) to create a new bot 
2. Name it give it a pfp etc
3. Now we are going to make this bot private (so that nobody can control your game except you). First step to doing this is going inside the "Installation" tab and setting the install link to "None"
4. Go to the Bot page and make sure the "Public Bot" is disabled
5. On the bot page give the bot the Presence, Server Members and Message Content Intents
6. Click Reset Token enter your MFA to get the bot token (Keep this somewhere safe if you share this to anybody they can hack your bot)
7. Go to the OAuth2 page, URL Generator, then give the "bot", "Send Messages", "Attach Files", "Read Mesasage History"
8. Copy the link and invite it to a private server (you should create a server just for yourself)
9. Paste the token you got earlier inside PROHack in the `Discord Options` menu inside the token field (don't forget to save) and press reboot bot
11. use the `!get-user-id` command to get your user id and paste it inside the user id field (don't forget to save)
12. use the `!set-announce-channel` to set the channel in which you want to recieve your messages (should be a private server channel)
13. from the Discort Options menu press the Send test message button to send a test message. If you recieved a message containing a screenshot of your desktop and some text then everything is working!
