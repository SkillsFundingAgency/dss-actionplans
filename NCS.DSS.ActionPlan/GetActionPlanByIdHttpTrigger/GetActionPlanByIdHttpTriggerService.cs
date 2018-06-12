﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger
{
    public class GetActionPlanByIdHttpTriggerService
    {
        public async Task<Models.ActionPlan> GetActionPlan(Guid actionPlanId)
        {
            var actionPlans = CreateTempActionPlans();
            var result = actionPlans.FirstOrDefault(a => a.ActionPlanId == actionPlanId);
            return await Task.FromResult(result);
        }

        public List<Models.ActionPlan> CreateTempActionPlans()
        {
            var actionPlanList = new List<Models.ActionPlan>
            {
                new Models.ActionPlan
                {
                    ActionPlanId = Guid.Parse("eb29f1e7-f4c3-43fd-baa8-daf3565b4855"),
                    InteractionId = Guid.NewGuid(),
                    DateActionPlanCreated = DateTime.UtcNow,
                    DateActionPlanSentToCustomer = DateTime.UtcNow,
                    DateActionPlanAcknowledged = DateTime.UtcNow,
                    PriorityCustomerId = 1,
                    CurrentSituation = "Unknown",
                    LastModifiedDate = DateTime.Today.AddYears(-1),
                    LastModifiedTouchpointId = Guid.NewGuid()
                },
                new Models.ActionPlan
                {
                    ActionPlanId = Guid.Parse("601769c5-3fe7-4db7-ba8b-41a4e1c0e4c5"),
                    InteractionId = Guid.NewGuid(),
                    DateActionPlanCreated = DateTime.UtcNow,
                    DateActionPlanSentToCustomer = DateTime.UtcNow,
                    DateActionPlanAcknowledged = DateTime.UtcNow,
                    PriorityCustomerId = 2,
                    CurrentSituation = "Pending",
                    LastModifiedDate = DateTime.Today.AddYears(-2),
                    LastModifiedTouchpointId = Guid.NewGuid()
                },
                new Models.ActionPlan
                {
                    ActionPlanId = Guid.Parse("c154d0f0-39fc-4a41-acb3-62e3260fe277"),
                    InteractionId = Guid.NewGuid(),
                    DateActionPlanCreated = DateTime.UtcNow,
                    DateActionPlanSentToCustomer = DateTime.UtcNow,
                    DateActionPlanAcknowledged = DateTime.UtcNow,
                    PriorityCustomerId = 3,
                    CurrentSituation = "Started",
                    LastModifiedDate = DateTime.Today.AddYears(-3),
                    LastModifiedTouchpointId = Guid.NewGuid()
                }
            };

            return actionPlanList;
        }
    }
}