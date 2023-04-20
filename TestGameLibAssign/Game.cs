using GameLibAssignment;
using GameLibAssignment.DecorationDesignPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGameLibAssign
{


    public class Game
    {
        private readonly World world;
        private readonly Player player;
        private readonly Enemy enemy;
        private bool playerTurn;



        public Game()
        {
            world = new World(20, 20);

            player = new Player("Player", 100, 2, 30);

            enemy = new Enemy("Enemy", 100, 10);

            world.AddCreature(player);
            world.AddCreature(enemy);


            AttackItem knife = new AttackItem("knife", 13);
            AttackItem sword = new AttackItem("sword", 18);
            AttackItem mace = new AttackItem("mace", 8);
            world.AddWorldObject(knife);
            world.AddWorldObject(sword);
            world.AddWorldObject(mace);

            AttackItem knife1 = new AttackItem("knife", 13);
            AttackItem sword1 = new AttackItem("sword", 18);
            AttackItem mace1 = new AttackItem("mace", 8);
            world.AddWorldObject(knife1);
            world.AddWorldObject(sword1);
            world.AddWorldObject(mace1);

            DefenceItem shield1 = new DefenceItem("wooden shield", 3);
            DefenceItem shield2 = new DefenceItem("stone shield", 4);
            DefenceItem shield3 = new DefenceItem("iron shield", 5);
            world.AddWorldObject(shield1);
            world.AddWorldObject(shield2);
            world.AddWorldObject(shield3);

            DefenceItem shield4 = new DefenceItem("wooden shield", 2);
            DefenceItem shield5 = new DefenceItem("stone shield", 3);
            DefenceItem shield6 = new DefenceItem("iron shield", 6);
            world.AddWorldObject(shield4);
            world.AddWorldObject(shield5);
            world.AddWorldObject(shield6);

            playerTurn = true;


        }


        public void Play()
        {
            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.Clear();
                world.Draw();

                player.DisplayPlayerInfo(player);
                enemy.DisplayEnemyInfo(enemy);




                if (playerTurn)
                {
                    PlayerTurn();
                }
                else
                {
                    EnemyTurn();
                }

                playerTurn = !playerTurn;
            }

            Console.Clear();
            world.Draw();

            if (player.Health <= 0)
            {
                Logger.Log("Game over. You died.");
            }
            else
            {
                Logger.Log("Congratulations. You won.");
            }

            Console.ReadLine();
        }

        private void PlayerTurn()
        {
            Logger.Log("Player's turn. Choose direction (w, a, s, d), press 'f' to attack,\npress 't' to use magic attack within 5 cells, or 'e' to pick up items: ");
            var key = Console.ReadKey(true).KeyChar;

            if (key == 'f' && Position.IsWithinOneCell(player.position, enemy.position))
            {
                player.Attack(enemy);

                Logger.Log($"You attacked {player.Damage}");
            }
            else if (key == 't' && Position.IsWithinNoOfCells(player.position, enemy.position, 5)) 
            {
                //decorator design pattern used here and adds a bit of magic damage to current damage and ability to attack from distance

                //since player class implements IMagicAttack interface it can be used as an IMagicAttck object and passed as a parameter
                //to AttackDecoratorPattern constructor
                
                IMagicAttack magicAttack = new AttackDecoratorPattern(player);
                magicAttack = new FireAttack(magicAttack, 3);
                if (player.Energy >= 10)
                {
                    magicAttack.MagicAttack(enemy);
                    Logger.Log($"You attacked with Fire attack {player.Damage}");
                }
                else
                {
                    Logger.Log("Not enough energy");
                }



            }
            else if (key == 'e')
            {
                PickUpItemsForPlayer();
            }
            else
            {
                player.Move(key, world);
            }
            Thread.Sleep(200);
        }

        private void EnemyTurn()
        {
            Logger.Log("Enemy's turn.");
            if (Position.IsWithinOneCell(player.position, enemy.position))
            {
                enemy.Attack(player);
                Logger.Log($"Enemy attacked {enemy.Damage}");
            }
            else
            {
                PickUpItemsForEnemy();
                enemy.EnemyRandomMovement(1, world);
            }

            Thread.Sleep(2000);
        }

        private void PickUpItemsForPlayer()
        {
            foreach (var obj in world.worldObjects.ToList())
            {
                if (Position.IsWithinOneCell(player.position, obj.position))
                {
                    if (obj is DefenceItem)
                    {
                        player.PickUp((DefenceItem)obj);
                        world.RemoveWorldObject(obj);
                        Logger.Log($"Player picked up {obj.Name}.");
                        player.Defence += obj.Defence;
                    }
                    else if (obj is AttackItem)
                    {
                        player.PickUp((AttackItem)obj);
                        world.RemoveWorldObject(obj);
                        Logger.Log($"Player picked up {obj.Name}.");
                        player.Damage += obj.Damage;
                    }
                }
            }
        }


        private void PickUpItemsForEnemy()
        {
            foreach (var obj in world.worldObjects.ToList())
            {
                if (Position.IsWithinOneCell(enemy.position, obj.position))
                {
                    // Have the enemy pick up the object automatically
                    if (obj is DefenceItem)
                    {
                        enemy.PickUp((DefenceItem)obj);
                        world.RemoveWorldObject(obj);
                        Logger.Log($"Enemy picked up {obj.Name}.");
                        enemy.Defence += obj.Defence;
                    }
                    else if (obj is AttackItem)
                    {
                        enemy.PickUp((AttackItem)obj);
                        world.RemoveWorldObject(obj);
                        Logger.Log($"Enemy picked up {obj.Name}.");
                        enemy.Damage += obj.Damage;
                    }
                }
            }
        }



    }


}
