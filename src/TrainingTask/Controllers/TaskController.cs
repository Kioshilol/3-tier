﻿using System;
using System.Collections.Generic;
using System.Linq;
using BLayer.DTO;
using BLayer.Interfaces;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TrainingTask.Models;
using TrainingTask.ViewModels;

namespace TrainingTask.Controllers
{
    public class TaskController : Controller
    {
        private ITaskService<TaskDTO> _taskService;
        private IService<ProjectDTO> _projectService;
        private IService<EmployeeDTO> _employeeService;
        private IMapper<TaskDTO, TaskViewModel> _taskMapper;
        private IMapper<ProjectDTO, ProjectViewModel> _projectMapper;
        private IMapper<EmployeeDTO, EmployeeViewModel> _employeeMapper;
        private ILogger<TaskController> _logger;
        public TaskController(ITaskService<TaskDTO> taskService, IService<ProjectDTO> projectService,
            IService<EmployeeDTO> employeeService, IMapper<ProjectDTO, ProjectViewModel> projectMapper,
            IMapper<EmployeeDTO, EmployeeViewModel> employeeMapper,
            IMapper<TaskDTO, TaskViewModel> taskMapper,
            ILogger<TaskController> logger)
        {
            _projectService = projectService;
            _employeeService = employeeService;
            _taskService = taskService;
            _taskMapper = taskMapper;
            _projectMapper = projectMapper;
            _employeeMapper = employeeMapper;
            _logger = logger;
        }

        public IActionResult Index(int page = 1)
        {
            _logger.LogInformation($"{page}");
            var tasksViewModelPaging = new List<TaskViewModel>();
            var taskListPaging = _taskService.GetAllWithPaging(page);

            foreach (var task in taskListPaging)
            {
                var taskViewModel = _taskMapper.Map(task);
                taskViewModel.Project = new ProjectViewModel();
                tasksViewModelPaging.Add(taskViewModel);
            }

            var tasksViewModel = new List<TaskViewModel>();
            var taskList = _taskService.GetAll();

            foreach(var task in taskList)
            {
                var taskViewModel = _taskMapper.Map(task);
                tasksViewModel.Add(taskViewModel);
            }

            var projectsDTO = _projectService.GetAll();
            var projectsViewModel = new List<ProjectViewModel>();

            foreach (var project in projectsDTO)
            {
                var projectViewModel = _projectMapper.Map(project);
                projectsViewModel.Add(projectViewModel);

                foreach (var task in tasksViewModelPaging)
                {
                    if(task.ProjectId == projectViewModel.Id)
                    {
                        task.Project = projectViewModel;
                    }
                }
            }

            var pageViewModel = new PageViewModel
            {
                PageNumber = page,
                RowsPerPage = PageSetting.GetRowsPerPage(),
                TotalRecords = tasksViewModel.Count,
                TotalPages = tasksViewModel.Count / PageSetting.GetRowsPerPage()
            };

            var indexViewModel = new IndexViewModel<TaskViewModel>
            {
                ViewModelList = tasksViewModelPaging,
                Page = pageViewModel
            };

            return View(indexViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation($"Create task");
            var employeeDTO = _employeeService.GetAll();
            var employeeViewModel = new List<EmployeeViewModel>();

            foreach (var employee in employeeDTO)
            {
                var employeeViewM = _employeeMapper.Map(employee);
                employeeViewModel.Add(employeeViewM);
            }

            ViewBag.employee = employeeViewModel;
            var projectsDTO = _projectService.GetAll();
            var projectsViewModel = new List<ProjectViewModel>();

            foreach (var project in projectsDTO)
            {
                var projectViewModel = _projectMapper.Map(project);
                projectsViewModel.Add(projectViewModel);
            }

            SelectList projects = new SelectList(projectsViewModel, "Id", "Name");
            ViewBag.Projects = projects;

            var model = new TaskViewModel
            {
                DateOfEnd = DateTime.UtcNow
            };

            return View(model);
        }

        [HttpGet("projects/{projectid}/Create")]
        public IActionResult Create(int projectid)
        {
            _logger.LogInformation($"{projectid}");
            if (projectid > 0)
            {
                var employeeDTO = _employeeService.GetAll();
                var employeeViewModel = new List<EmployeeViewModel>();

                foreach (var employee in employeeDTO)
                {
                    var employeeViewM = _employeeMapper.Map(employee);
                    employeeViewModel.Add(employeeViewM);
                }

                ViewBag.Employee = employeeViewModel;

                var model = new TaskViewModel()
                {
                    DateOfEnd = DateTime.UtcNow
                };

                var projectDto = _projectService.GetById(projectid);
                if (projectDto != null)
                {
                    model.ProjectId = projectid;
                    model.Project = _projectMapper.Map(projectDto);

                    return View(model);
                }
            }
            return new BadRequestResult();
        }

        [HttpPost]
        public IActionResult Create(TaskViewModel task, int[] selectedEmployee)
        {
            _logger.LogInformation($"{task}; ids of selected employees: {selectedEmployee}");

            if (ModelState.IsValid)
            {
                foreach(var id in selectedEmployee)
                {
                    task.EmployeeTasks.Add(new EmployeeTasksViewModel { EmployeeId = id });
                }

                var taskDTO = _taskMapper.Map(task);
                _taskService.Add(taskDTO);
                return RedirectToAction("Index");
            }
            else
            {
                return View(task);
            }
        }
        public IActionResult Edit(int? id)
        {
            _logger.LogInformation($"{id}");

            if (id != null)
            {
                var employeeDTO = _employeeService.GetAll();
                var employeeViewModel = new List<EmployeeViewModel>();

                foreach (var employee in employeeDTO)
                {
                    var employeeViewM = _employeeMapper.Map(employee);
                    employeeViewModel.Add(employeeViewM);
                }

                ViewBag.Employee = employeeViewModel;

                var projectsDTO = _projectService.GetAll();
                var projectsViewModel = new List<ProjectViewModel>();

                foreach (var project in projectsDTO)
                {
                    var projectViewModel = _projectMapper.Map(project);
                    projectsViewModel.Add(projectViewModel);
                }

                SelectList projects = new SelectList(projectsViewModel, "Id", "Name");
                ViewBag.Projects = projects;
                var taskDTO = _taskService.GetById(id.Value);
                taskDTO.DateOfEnd = DateTime.Now;
                var taskModelView = _taskMapper.Map(taskDTO);
                return View(taskModelView);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(TaskViewModel task, int[] selectedEmployee)
        {
            _logger.LogInformation($"{task}; ids of selected employees: {selectedEmployee}");

            if (ModelState.IsValid)
            {
                foreach (var id in selectedEmployee)
                {
                    task.EmployeeTasks.Add(new EmployeeTasksViewModel { EmployeeId = id });
                }

                var taskDTO = _taskMapper.Map(task);
                _taskService.Edit(taskDTO);
                return RedirectToAction("Index");
            }
            else
                return View(task);
        }

        public IActionResult Delete(int? id)
        {
            _logger.LogInformation($"{id}");

            if (id != null)
            {
                var taskDTO = _taskService.GetById(id.Value);
                if(taskDTO != null)
                {
                    var taskViewModel = _taskMapper.Map(taskDTO);
                    return View(taskViewModel);
                }
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation($"{id}");
            try
            {
                _taskService.Delete(id);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message, "Stopped program because of exception ");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            _logger.LogInformation($"{id}");

            if (id != null)
            {
                var taskDTO = _taskService.GetById(id.Value);
                if(taskDTO != null)
                {
                    var taskModelView = _taskMapper.Map(taskDTO);
                    return View(taskModelView);
                }
            }

            return NotFound();
        }

        public IActionResult UploadToXML()
        {
            try
            {
                _taskService.ExportToXML();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "Stopped program because of exception ");
            }
            return RedirectToAction("Index");
        }

        public IActionResult UploadToExcel()
        {
            try
            {
                _taskService.ExportToExcel();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "Stopped program because of exception ");
            }

            return RedirectToAction("Index");
        }
    }
}