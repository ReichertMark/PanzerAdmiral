using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Demos
{
    class EnemyTrigger : Trigger
    {
        BombingEnemy enemy1;
        BombingEnemy enemy2;
        public Vector2 spawnPos;

        /// <summary>
        /// Create Enemy Trigger
        /// </summary>
        /// <param name="setPosition">set Trigger Position</param>
        /// <returns></returns>
        public EnemyTrigger(Vector2 setPosition)
            : base(setPosition) 
        {

            // AI uses Display units for positioning
            spawnPos = ConvertUnits.ToDisplayUnits(new Vector2(Position.X, Position.Y));

            // first enemy is upper right
            spawnPos.Y -= 530;//300;
            spawnPos.X += 700;
            enemy1 = new BombingEnemy(spawnPos, GameSettings.BombingEnemyHitPoints);

            // second gets some x-offset
            //spawnPos.Y -= 50;
            spawnPos.X += 450;
            enemy2 = new BombingEnemy(spawnPos, GameSettings.BombingEnemyHitPoints);
        }

        /// <summary> Just give the Enemys some speed values </summary>
        public override void DoEventLogic()
        {
            enemy1.Speed = 3f;
            enemy2.Speed = 3f;

            //base.DoEventLogic();
        } // DoEventLogic()
    } // class EnemyTrigger
}
