using System;
using Autofac;
using Common.Events;
using Common.Test;
using Infrastructure.DataModel.Events;
using Xunit;

namespace EventSystemWebApi.Test
{
    public class EventServiceTests : TestBase
    {
        protected override void BuildUp(ContainerBuilder builder)
        {
        }

        [Fact]
        public void UpdateEvent_CopyNulls()
        {
            var now = DateTime.Now;
            const string NAME = "This is the old name";

            var oldEvent = new Event
            {
                Budget = 100,
                End = now,
                Start = now,
                Name = NAME
            };

            var newEvent = new Event
            {
                Budget = null,
                End = now,
                Start = now,
                Name = ""
            };

            EventService.UpdateEvent(oldEvent, newEvent);

            Assert.Equal(null, oldEvent.Budget);
            Assert.Equal(now, oldEvent.End);
            Assert.Equal(now, oldEvent.Start);
            Assert.Equal("", oldEvent.Name);
        }

        [Fact]
        public void UpdateEvent_Everything()
        {
            var oldEvent = new Event
            {
                Budget = 100,
                End = DateTime.Now,
                Start = DateTime.Now,
                Name = "This is the old name"
            };

            var newEvent = new Event
            {
                Budget = 200,
                End = DateTime.Now.AddDays(-2),
                Start = DateTime.Now.AddDays(-2),
                Name = "This is the new name"
            };

            EventService.UpdateEvent(oldEvent, newEvent);

            Assert.Equal(newEvent.Budget, oldEvent.Budget);
            Assert.Equal(newEvent.End, oldEvent.End);
            Assert.Equal(newEvent.Start, oldEvent.Start);
            Assert.Equal(newEvent.Name, oldEvent.Name);
        }
    }
}