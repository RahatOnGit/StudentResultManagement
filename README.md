Alhamdulillah, by the blessings of Allah, I have completed my first project using Asp.Net Core MVC.


# Student Result Mangement System

About
-----

- The application can be used for generating students result easily.


Technologies used here are
---------------------------

Backend: Php, Laravel, Laravel Rest API

Frontend: HTML, CSS, Javascript, BootStrap

Database: MySQL


Explanation
-----------


- User Registration, login and logout is handled by Breeze(for web) and Sanctum(for API) packages.

- After properly using the packages, we will get "users" table in the database.

- "users" table will contain the necessary details of the users.

- For storing all tasks data, I have used a table named "all_tasks". I have added 6 fields to the table. These fields are 
 'user_id' (Foreign Key), 'taskname', 'details'(nullable), 'date' , 'time' and 'status'. 

- 'user_id' is the Primary key of the "users" table.

- To use the application, user should be authenticated. So, for every request user authentication
  is checked.

- If an authenticated user wants to add a new task, this task data will be added in the 
  "all_tasks" table. Authenticated user id will be set as the value of the 'user_id' field.

- If an authenticated user wants to get all his tasks, a query is made using the
 authenticated user id at the "all_tasks" table.

- To edit a task, authenticated user id is matched with the 'user_id' field for the task. If it 
  is matched only, then the user can edit the task.

- To delete a task, authenticated user id is matched with the 'user_id' field for the task. If it 
  is matched only, then the user can delete the task.

- All the tasks data for a user is sorted in ascending order by date and time, then it is served.

- For filtering, a query is made using the status type at the user's all tasks.


API Development
---------------

 For the interaction with other applications, an API for the Task Management System has also been developed.
 
- Route for the API: https://github.com/RahatOnGit/Task-Management-System/blob/main/routes/api.php
- Controller for the API: https://github.com/RahatOnGit/Task-Management-System/blob/main/app/Http/Controllers/TaskApiController.php

Simple Demo:
-------------

*     Home Page

![home](https://github.com/user-attachments/assets/b7650127-3e0a-4c00-be78-a423fcd68b8b)



