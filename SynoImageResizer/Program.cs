using System;
using System.IO;
using System.Linq;
using ImageResizer4DotNet;
using Path = Fluent.IO.Path;

namespace SynoImageResizer
{
    class Program
    {
        /*
        #!/bin/bash
        pushd "$1"
        shopt -s nocaseglob
        if [ ! -d @eaDir ] ; then mkdir @eaDir ; fi
        for f in *.jpg ; do
        if [ "$f" == "*.jpg" ] ; then break ; fi
        echo "$1 - $f..."
        if [ ! -d @eaDir/$f ] ; then mkdir @eaDir/$f ; fi 
        if [ ! -f @eaDir/$f/SYNOPHOTO:THUMB_XL.jpg ] ; then convert $f -resize 1280x1280\> -quality 90 -unsharp 0.5x0.5+1.25+0.0 @eaDir/$f/SYNOPHOTO:THUMB_XL.jpg ; fi
        if [ ! -f @eaDir/$f/SYNOPHOTO:THUMB_L.jpg ] ; then convert @eaDir/$f/SYNOPHOTO:THUMB_XL.jpg -resize 800x800\> -quality 90 -unsharp 0.5x0.5+1.25+0.0 @eaDir/$f/SYNOPHOTO:THUMB_L.jpg ; fi
        if [ ! -f @eaDir/$f/SYNOPHOTO:THUMB_M.jpg ] ; then convert @eaDir/$f/SYNOPHOTO:THUMB_L.jpg -resize 320x320\> -quality 90 -unsharp 0.5x0.5+1.25+0.0 @eaDir/$f/SYNOPHOTO:THUMB_M.jpg ; fi
        if [ ! -f @eaDir/$f/SYNOPHOTO:THUMB_S.jpg ] ; then convert @eaDir/$f/SYNOPHOTO:THUMB_M.jpg -resize 120x120\> -quality 90 -unsharp 0.5x0.5+1.25+0.0 @eaDir/$f/SYNOPHOTO:THUMB_S.jpg ; fi
        if [ ! -f @eaDir/$f/SYNOPHOTO:THUMB_B.jpg ] ; then convert @eaDir/$f/SYNOPHOTO:THUMB_L.jpg -resize 640x640\> -quality 90 -unsharp 0.5x0.5+1.25+0.0 @eaDir/$f/SYNOPHOTO:THUMB_B.jpg ; fi
        done
        popd
        */
        static void Main(string[] args)
        {
            var jpegImagesPath = Path.Current.Combine("@test").Files("*.jpg", false).ToList();
            //var jpegImagesPath = Path.Get(@"\\homediskstation\Photo\Todelete").Files("*.jpg", false).ToList();

            var eaDir = Path.Current.Combine("@test").CreateSubDirectory("@eaDir");
            //var eaDir = Path.Get(@"\\homediskstation\Photo\Todelete").CreateSubDirectory("eaDir");

            foreach (var jpegImagePath in jpegImagesPath)
            {
                CreateThumbnails(jpegImagePath, eaDir);
            }
        }

        private static void CreateThumbnails(Path jpegImagePath, Path eaDir)
        {
            using (var original = new MemoryStream(File.ReadAllBytes(jpegImagePath.FullPath)))
            {
                Console.WriteLine("Resizing 120: {0}", jpegImagePath.FileName);
                Resize(jpegImagePath, eaDir.Combine(jpegImagePath.FileName).CreateDirectory(), 120, "SYNOPHOTO-THUMB_S.jpg", original);
                original.Position = 0;

                Console.WriteLine("Resizing 320: {0}", jpegImagePath.FileName);
                Resize(jpegImagePath, eaDir.Combine(jpegImagePath.FileName).CreateDirectory(), 320, "SYNOPHOTO-THUMB_M.jpg", original);
                original.Position = 0;

                Console.WriteLine("Resizing 640: {0}", jpegImagePath.FileName);
                Resize(jpegImagePath, eaDir.Combine(jpegImagePath.FileName).CreateDirectory(), 640, "SYNOPHOTO-THUMB_B.jpg", original);
                original.Position = 0;

                Console.WriteLine("Resizing 800: {0}", jpegImagePath.FileName);
                Resize(jpegImagePath, eaDir.Combine(jpegImagePath.FileName).CreateDirectory(), 800, "SYNOPHOTO-THUMB_L.jpg", original);
            }
        }

        private static async void Resize(Path jpegImagePath, Path createDirectory, int size, string filename, MemoryStream original)
        {
            var result = Resizer.LowJpeg(original, size, size).ToArray();

            using (var sourceStream = File.Open(createDirectory.Combine(filename).FullPath, FileMode.Create))
            {
                sourceStream.Seek(0, SeekOrigin.End);
                await sourceStream.WriteAsync(result, 0, result.Length);
            }

            Console.WriteLine("Resized {0}: {1}", size, jpegImagePath.FileName);
        }

    }
}
