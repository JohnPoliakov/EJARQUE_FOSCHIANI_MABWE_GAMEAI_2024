﻿using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using CommonAPI.TreeBehaviour;
using CommonAPI.Actions;
using UnityEngine.Assertions;
using CommonAPI.Conditions;
using CommonAPI;
using System;
using UnityEngine;

namespace EJARQUE
{
    public class Skynet
    {
        public List<AIAction> ComputeAIDecision(int myID, List<PlayerInformations> playerInfos)
        {
            List<AIAction> actionList = new List<AIAction>();
            PlayerInformations myPlayerInfos = GetPlayerInfos(myID, playerInfos);

            PlayerInformations target = SelectTarget(playerInfos, myPlayerInfos);

            if (target == null)
                return actionList;

            SelectorNode root = new SelectorNode();

            SequenceNode attackSequence = new SequenceNode();
            attackSequence.AddChild(new LookAtTargetNode(target.Transform.Position));
            attackSequence.AddChild(new FireAtTargetNode());

            SequenceNode moveToTargetSequence = new SequenceNode();
            moveToTargetSequence.AddChild(new LookAtTargetNode(target.Transform.Position));
            moveToTargetSequence.AddChild(new MoveToTargetNode(target.Transform.Position));

            // Conditions
            ConditionNode isTargetInRange = new IsTargetInRangeNode(target, 5.0f);


            // Move to target if not in range
            SequenceNode moveToTargetConditionSequence = new SequenceNode();
            moveToTargetConditionSequence.AddChild(new LookAtTargetNode(target.Transform.Position));
            moveToTargetConditionSequence.AddChild(isTargetInRange);
            moveToTargetConditionSequence.AddChild(moveToTargetSequence);

            // Compile Move and Shoot
            SequenceNode moveAttack = new SequenceNode();
            moveAttack.AddChild(attackSequence);
            moveAttack.AddChild(moveToTargetConditionSequence);


            root.AddChild(moveAttack);

            root.Execute(target, actionList);

            return actionList;
        }

        private PlayerInformations SelectTarget(List<PlayerInformations> playerInfos, PlayerInformations myPlayerInfos)
        {
            PlayerInformations targetData = null;
            float targetDistance = float.MaxValue;
            float targetHealth = float.MaxValue;

            foreach (PlayerInformations playerInfo in playerInfos)
            {
                if (!playerInfo.IsActive)
                    continue;

                if (playerInfo.PlayerId == myPlayerInfos.PlayerId)
                    continue;

                

                if (Vector3.Distance(playerInfo.Transform.Position, playerInfo.Transform.Position) < targetDistance)
                {
                    targetData = playerInfo;
                    targetDistance = Vector3.Distance(playerInfo.Transform.Position, playerInfo.Transform.Position);
                }

                if(targetHealth > playerInfo.CurrentHealth)
                {
                    targetHealth = playerInfo.CurrentHealth;
                    targetData = playerInfo;
                }

                //return playerInfo;
            }
            return targetData;
        }

        public PlayerInformations GetPlayerInfos(int parPlayerId, List<PlayerInformations> parPlayerInfosList)
        {
            foreach (PlayerInformations playerInfo in parPlayerInfosList)
            {
                if (playerInfo.PlayerId == parPlayerId)
                    return playerInfo;
            }

            Assert.IsTrue(false, "GetPlayerInfos : PlayerId not Found");
            return null;
        }
    }


}