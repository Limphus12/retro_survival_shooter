using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class GameManager : MonoBehaviour
    {
        private GameObject player;

        public static GameObject Player;
        public static PlayerController PlayerController;
        public static PlayerStats PlayerStats;
        public static PlayerInventory PlayerInventory;
        public static PlayerInteraction PlayerInteraction;

        private void Awake() => Init();

        private void Init()
        {
            if (!player) player = GameObject.FindGameObjectWithTag("Player");

            Player = player;

            PlayerController = Player.GetComponent<PlayerController>();
            PlayerStats = Player.GetComponent<PlayerStats>();
            PlayerInventory = Player.GetComponent<PlayerInventory>();
            PlayerInteraction = Player.GetComponent<PlayerInteraction>();
        }
    }
}