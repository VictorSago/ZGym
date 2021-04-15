using System.Linq;
using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ZGym.Core.Entities;
using ZGym.Core.Repositories;
using ZGym.Core.ViewModels;
using ZGym.Data.Data;
using ZGym.Tests.Extensions;
using ZGym.Web.Controllers;

namespace ZGym.Tests.Controller
{
    [TestClass]
    public class GymClassesTests
    {
        private GymClassesController controller;
        private Mock<IGymClassRepository> mockGymClassRepo;
        private Mapper mapper;

        [TestInitialize]
        public void SetUp()
        {
            mockGymClassRepo = new Mock<IGymClassRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.GymClassRepository).Returns(mockGymClassRepo.Object);

            mapper = new Mapper(new MapperConfiguration(cfg => 
            {
                var profile = new MapperProfile();
                cfg.AddProfile(profile);
            }));

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            controller = new GymClassesController(mockUserManager.Object, mapper, mockUoW.Object);
        }

        [TestMethod]
        public void Index_NotAuthenticated_ReturnsExpected()
        {
            var gymClasses = GetGymClassList();
            var expected = mapper.Map<IndexViewModel>(gymClasses);

            controller.SetUserIsAuthenticated(false);
            mockGymClassRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(gymClasses);

            var vm = new IndexViewModel 
            {
                ShowHistory = false
            };
            var viewResult = controller.Index(vm).Result as ViewResult;
            var actual = (IndexViewModel)viewResult.Model;

            Assert.AreEqual(expected.GymClasses.Count(), actual.GymClasses.Count());
        }

        private List<GymClass> GetGymClassList()
        {
            return new List<GymClass>
            {
                new GymClass
                {
                    Id = 1,
                    Name = "Spinning",
                    Description = "Easy",
                    StartTime = DateTime.Today.AddDays(3) + new TimeSpan(13, 0, 0),
                    Duration = new TimeSpan(0,60,0)
                },
                new GymClass
                {
                    Id = 2,
                    Name = "Salsa",
                    Description = "Beginners",
                    StartTime = DateTime.Today.AddDays(-3) + new TimeSpan(18, 0, 0),
                    Duration = new TimeSpan(1,15,0)
                }
            };
        }
    }
}