﻿using UnityEngine;

namespace MiniJameGam9.Character
{
    public class Profile
    {
        public Profile(bool isAi, string name, Camera camera = null, InputContainer container = null)
            => (IsAi, Name, Kill, Death, DamageDealt, Camera, Container)
            = (isAi, name, 0, 0, 0, camera, container);

        public bool IsAi { get; set; }
        public string Name { get; set; }
        public int Kill { get; set; }
        public int Death { get; set; }
        public int DamageDealt { get; set; }
        public Camera Camera { get; set; }
        public InputContainer Container { get; set; }
    }
}
