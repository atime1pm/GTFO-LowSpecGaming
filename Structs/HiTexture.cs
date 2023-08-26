using ItemSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Structs
{
    internal class HiTexture
    {
        string name;
        int index;
        string firstPath;
        public List<Texture2D> texture;
        public HiTexture(string path) {
            name = "";
            index = 0;
            firstPath = path;
            texture = new List<Texture2D>();
            texture.Add(LoadTexture(path));
        }
        private Texture2D LoadTexture(string path) {
            Texture2D newSight = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            byte[] data = File.ReadAllBytes(path);
            newSight.LoadImage(data);
            return newSight;
        }
        public Texture2D GetFirstTexture() {
            Texture2D newSight = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            byte[] data = File.ReadAllBytes(firstPath);
            newSight.LoadImage(data);
            return newSight;
        }
        public Texture2D GetTexture(int index) { 
            return texture[index];
        }
    }
}
