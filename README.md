[![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://azuredeploy.net/)

Main Technologies:

	- Asp.net Core 1.0.0-rc1
	- Angularjs 2

Project structure

````
	ImageUploadDemo							    -- application
		|- controllers
		|	|- UploadController.cs				-- public api
		|
		|- ImageProcessProvider
			|- AzureBlobProvider.cs				-- store raw image onto Azure Blob Storage
			|- LocalFileProvider.cs				-- store raw image under "wwwroot/dl" folder

	ImageUploadDemo.Test						-- unit tests
````

Hightlight:

	Currently there is not image library avaiable for DNX, since there is no "System.Draw" function implemented
	Image resize function is relying on Google CDN which has limited on file size

How to open and run project locally:

	- On Windows:
		Install Microsoft Visual Studio Community 2015 with Update 1
		Open ImageUploadDemo.sln
		Open Startup.cs make sure it is running with "AzureBlobProvider"
			services.AddTransient<IImageProvider, AzureBlobProvider>();
		Open appsettings.json to fill in StorageAccount and StorageKey
		F5 to run application
		
	- On Linux:
		TODO (should runable since asp.net Core 1.0 is corss platform)

Deploy to Azure App Service

	- Set local git https://azure.microsoft.com/en-us/documentation/articles/web-sites-publish-source-control/
	- Push solution to git repo
