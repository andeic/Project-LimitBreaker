﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;
/// <summary>
/// Summary description for ScheduleManager
/// </summary>
public class ScheduleManager
{
    public ScheduleManager()
    {

    }

    public List<ScheduledRoutine> getRoutines()
    {
        using (var context = new Layer2Container())
        {
            return context.ScheduledRoutines.OrderBy(o => o.startTime).ToList();
        }
    }

    public List<scheduledItem> getScheduledItemsByDay(Int32 userID, DateTime day)
    {
        using (var context = new Layer2Container())
        {
            var ruleDate = Convert.ToDateTime(day).Date;
            var routines = from r in context.ScheduledRoutines
                           orderby r.startTime
                           where (r.LimitBreaker.id == userID && r.startTime.Day == day.Day)
                           select new scheduledItem
                           {
                               itemName = r.Routine.name,
                               startTime = r.startTime,
                               user = r.LimitBreaker,
                               id = r.id,
                               isExericse = false
                           };
            var exercises = from e in context.ScheduledExercises
                            orderby e.startTime
                            where (e.LimitBreakers.id == userID && e.startTime.Day == day.Day)
                            select new scheduledItem
                            {
                                itemName = e.Exercise.name,
                                startTime = e.startTime,
                                user = e.LimitBreakers,
                                id = e.id,
                                isExericse = true
                            };
            var items = routines.Concat(exercises).ToList();

            return items.ToList();
        }
    }

    public List<scheduledItem> getScheduledItemsByDayOfTheYear(Int32 userID, DateTime day)
    {
        using (var context = new Layer2Container())
        {
            var ruleDate = Convert.ToDateTime(day).Date;
            var routines = from r in context.ScheduledRoutines
                           orderby r.startTime
                           where (r.LimitBreaker.id == userID && r.startTime.Day == day.Day)
                           select new scheduledItem
                           {
                               itemName = r.Routine.name,
                               startTime = r.startTime,
                               user = r.LimitBreaker,
                               id = r.id,
                               isExericse = false
                           };
            var exercises = from e in context.ScheduledExercises
                            orderby e.startTime
                            where (e.LimitBreakers.id == userID && e.startTime.Day == day.Day)
                            select new scheduledItem
                            {
                                itemName = e.Exercise.name,
                                startTime = e.startTime,
                                user = e.LimitBreakers,
                                id = e.id,
                                isExericse = true
                            };
            var items = routines.Concat(exercises).ToList();

            return items.ToList();
        }
    }


    public bool scheduleNewRoutine(Int32 routineID, DateTime start, Int32 userID, bool notification)
    {
        bool rc = false;
        using (var context = new Layer2Container())
        {
            LimitBreaker lb = context.LimitBreakers.Where(x => x.id == userID).FirstOrDefault();
            if (lb != null)
            {
                List<scheduledItem> scheduledItemsForThatDay = new List<scheduledItem>();
                ScheduledRoutine newScheduledRoutine = new ScheduledRoutine();

                //This part is for validating if the exercise can be scheduled for a certain time
                /*scheduledItemsForThatDay = getScheduledItemsByDay(userID, start);
                foreach (var item in scheduledItemsForThatDay)
                {
                    if (item != null && start.AddHours(-1) <= item.startTime && start.AddHours(1) >= item.startTime)
                    {
                        return false;
                    }
                }
                */

                Routine routine = context.Routines.Where(e => e.id == routineID).FirstOrDefault();
                newScheduledRoutine.Routine = routine;
                newScheduledRoutine.startTime = start;
                newScheduledRoutine.LimitBreaker = lb;
                newScheduledRoutine.needEmailNotification = notification;
                context.ScheduledRoutines.AddObject(newScheduledRoutine);
                context.SaveChanges();
                rc = true;
            }
            return rc;
        }
    }

    public bool scheduleNewExercise(Int32 exerciseID, DateTime start, Int32 userID, bool notification)
    {
        bool rc = false;

        using (var context = new Layer2Container())
        {


            LimitBreaker lb = context.LimitBreakers.Where(x => x.id == userID).FirstOrDefault();
            if (lb != null)
            {
                List<scheduledItem> scheduledItemsForThatDay = new List<scheduledItem>();
                ScheduledExercise newScheduledExercise = new ScheduledExercise();

                //This part is for validating if the exercise can be scheduled for a certain time
                /* scheduledItemsForThatDay = getScheduledItemsByDay(userID, start);
                foreach (var item in scheduledItemsForThatDay)
                {
                    if (item != null && start.AddHours(-1) <= item.startTime && start.AddHours(1) >= item.startTime)
                    {
                        return false;
                    }
                }
                */
                Exercise exercise = context.Exercises.Where(e => e.id == exerciseID).FirstOrDefault();
                newScheduledExercise.Exercise = exercise;
                newScheduledExercise.startTime = start;
                newScheduledExercise.LimitBreakers = lb;
                newScheduledExercise.needEmailNotification = notification;
                context.ScheduledExercises.AddObject(newScheduledExercise);
                context.SaveChanges();
                rc = true;
            }
            return rc;
        }
    }

    //temp user for testing
    public List<LimitBreaker> getUsers()
    {
        using (var context = new Layer2Container())
        {
            return context.LimitBreakers.OrderBy(o => o.id).ToList();
        }
    }

    public LimitBreaker getUser(int userID)
    {
        using (var context = new Layer2Container())
        {
            context.ContextOptions.LazyLoadingEnabled = false;

            return context.LimitBreakers.FirstOrDefault(x => x.id == userID);

        }
    }

    public bool deletecheduledItem(Int32 itemID, bool isExercise, Int32 userID)
    {
        bool result = false;
        using (var context = new Layer2Container())
        {
            //Routine rc = new Routine();
            try
            {
                if (isExercise)
                {
                    ScheduledExercise rc = context.ScheduledExercises.Where(e => e.id == itemID).FirstOrDefault();
                    if (rc != null)
                    {
                        context.ScheduledExercises.DeleteObject(rc);
                        context.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    ScheduledRoutine rc = context.ScheduledRoutines.Where(e => e.id == itemID).FirstOrDefault();
                    if (rc != null)
                    {
                        context.ScheduledRoutines.DeleteObject(rc);
                        context.SaveChanges();
                        result = true;

                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                // write off the execeptions to my error.log file
                StreamWriter wrtr = new StreamWriter(System.Web.HttpContext.Current.ApplicationInstance.Server.MapPath("~/assets/documents/" + @"\" + "error.log"), true);

                wrtr.WriteLine(DateTime.Now.ToString() + " | Error: " + e);

                wrtr.Close();
            }

        }

        return result;
    }

    public bool modifyScheduledItem(Int32 id, Int32 newItemID, bool isExercise, DateTime date)
    {
        bool result = false;
        using (var context = new Layer2Container())
        {
            try
            {
                if (isExercise)
                {
                    ScheduledExercise rc = context.ScheduledExercises.Where(e => e.id == id).FirstOrDefault();
                    if (rc != null)
                    {
                        rc.Exercise = context.Exercises.Where(e => e.id == newItemID).FirstOrDefault();
                        rc.startTime = date;
                        context.ScheduledExercises.ApplyCurrentValues(rc);
                        context.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    ScheduledRoutine rc = context.ScheduledRoutines.Where(e => e.id == id).FirstOrDefault();
                    if (rc != null)
                    {
                        rc.Routine = context.Routines.Where(e => e.id == newItemID).FirstOrDefault();
                        rc.startTime = date;
                        context.ScheduledRoutines.ApplyCurrentValues(rc);
                        context.SaveChanges();
                        result = true;

                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        return result;
    }

}