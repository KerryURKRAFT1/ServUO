
   ROB INSTALL

   Download Server files at branch ServUO-UPDATED (https://github.com/KerryURKRAFT1/ServUO/tree/ServUO-UPDATED)

	Copy your client into /Client

	We will be using 5.0.8.3 but for testing you can use any client although I recommend staying with or
 	below 7.0.24

	Load up Classic UO with the same client (but not the same folder)

	Adjust /Config files that start with Custom-*.cfg

   WINDOWS

	Compile with Compile.WIN - Debug.bat

	Execute ServUO.exe

   LINUX

	 make debug

	./ServUO.sh (chmod +x if necessary)

   Linux Dependencies

   I use Debian 12 here is my list note: this covers ModernUO/ServUO/RunUO so you may not need everything

	Dotnet SDK
	Mono-Complete

	zlib1g-dev 
	libicu-dev 
	libz-dev 
	zstd
	libargon2-dev 
	tzdata 
	libdeflate-dev

   Note: Ignore [DEBUG] console messages, these are Kerry's stability tests
