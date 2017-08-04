var gulp = require("gulp");
var replace = require('gulp-replace');
var fs = require('fs');
var assemblyInfo = require('gulp-dotnet-assembly-info');
var argv = require('yargs').argv;

var version = new function () {
    // Change these to set the assembly version
    this.releaseComment = "alpha";
    this.major = 0;
    this.minor = 4;
    this.patch = 0; // changing this will reset the build number in local

    // do not assign. 
    this.build = argv.buildNumber === undefined ? -1 : argv.buildNumber;
    this.compose = function () { return this.major + "." + this.minor + "." + this.patch + "." + this.build }
};

/*************************************************************
 * Versioning
 * triggered via gulp script params:
 * e.g.  gulp prod --setVersion
 *       gulp prod --setVersion --buildNumber=[BUILD_NUMBER from VS BUILD]
 *************************************************************/
gulp.task("version", ["version:comment"], () => {
    if (argv.setVersion === undefined) { return; }

    saveInfo(getInfo("../Fabric.Terminology.API/properties/"));
    saveInfo(getInfo("../Fabric.Terminology.Domain/properties/"));
    saveInfo(getInfo("../Fabric.Terminology.SqlServer/properties/"));

    function getInfo(infoFilePath) {
        var infoFile = infoFilePath + "AssemblyInfo.cs";
        var fileContents = fs.readFileSync(infoFile, "utf8");
        var assembly = assemblyInfo.getAssemblyMetadata(fileContents);
        var fileVersion = assembly.AssemblyVersion.split(".");
        assembly.parsedVersion = {
            major: fileVersion[0] * 1,
            minor: fileVersion[1] * 1,
            patch: fileVersion[2] * 1,
            build: fileVersion[3] * 1
        };
        assembly.savePath = infoFilePath;
        return assembly;
    }

    function saveInfo(ddlInfo) {
        if (version.patch !== ddlInfo.parsedVersion.patch || version.build === -1) {
            version.build = ddlInfo.parsedVersion.build;
        }
        var info = {
            title: "Fabric.Terminology",
            description: "Service to provide shared healthcare terminology data",
            configuration: "",
            company: "Health Catalyst",
            product: "Fabric.Terminology",
            copyright: "Copyright " + new Date().getFullYear() + " \u00A9 Health Catalyst",
            trademark: "",
            culture: "",
            version: version.compose(),
            fileVersion: version.compose(),
            guid: ddlInfo.Guid
        };
        var infoFile = ddlInfo.savePath + "AssemblyInfo.cs";
        gulp.src(infoFile)
            .pipe(assemblyInfo(info))
            .pipe(gulp.dest(ddlInfo.savePath));
    }
});

gulp.task("version:comment",
    () => {
        if (argv.setVersion === undefined) { return; }
        gulp.src("../Fabric.Terminology.API/Configuration/TerminologyVersion.cs")
            .pipe(replace(/CurrentComment *(=>) *(.+)/g, "CurrentComment => \"" + version.releaseComment + "\";"))
            .pipe(gulp.dest("../Fabric.Terminology.API/Configuration/"));
    });