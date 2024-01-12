using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Data.Migrations;
using Proiect.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text.Encodings.Web;

namespace Proiect.Controllers {
    public class CppCompilerService {
        public CompilerResult CompileCode(string code) {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            string sourceFile = Path.Combine(tempDirectory, "temp.cpp");
            string outputFile = Path.Combine(tempDirectory, "temp.exe");
            string errorFile = Path.Combine(tempDirectory, "errors.txt");

            CompilerResult result = new CompilerResult();

            try {
                // Save the code to a temporary file
                File.WriteAllText(sourceFile, code);

                // Compile the code using g++
                ProcessStartInfo startInfo = new ProcessStartInfo("g++");
                startInfo.Arguments = $"-o \"{outputFile}\" \"{sourceFile}\"";
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;


                using (Process process = Process.Start(startInfo)) {
                    process.WaitForExit();
                     
                    if (process.ExitCode == 0) {
                        // Compilation succeeded
                        result.Success = true;

                        ProcessStartInfo executeStartInfo = new ProcessStartInfo(outputFile);
                        executeStartInfo.RedirectStandardOutput = true;
                        executeStartInfo.UseShellExecute = false;
                        executeStartInfo.CreateNoWindow = true;

                        using (Process executeProcess = Process.Start(executeStartInfo)) {
                            result.OutputMessage += executeProcess.StandardOutput.ReadToEnd();
                            executeProcess.WaitForExit();
                        }
                    } else {
                        // Compilation failed, read the error messages
                        result.Success = false;
                        List<string> errorMessages = new List<string>();
                        string line;
                        using (StreamReader reader = process.StandardError) {
                            while ((line = reader.ReadLine()) != null) {
                                errorMessages.Add(line);
                            }
                        }
                        result.ErrorMessages = errorMessages;
                    }
                }
            } catch (Exception ex) {
                // Exception occurred during compilation
                result.Success = false;
                result.ErrorMessages = new List<string> { ex.Message };
            } finally {
                // Clean up temporary files
                if (File.Exists(sourceFile)) {
                    File.Delete(sourceFile);
                }
                if (File.Exists(outputFile)) {
                    File.Delete(outputFile);
                }
                if (File.Exists(errorFile)) {
                    File.Delete(errorFile);
                }
                Directory.Delete(tempDirectory);
            }

            return result;
        }
    }

    public class PythonCompilerService {
        public CompilerResult CompileCode(string code) {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            string sourceFile = Path.Combine(tempDirectory, "temp.py");
            string outputFile = Path.Combine(tempDirectory, "output.txt");
            string errorFile = Path.Combine(tempDirectory, "errors.txt");

            CompilerResult result = new CompilerResult();

            try {
                // Save the code to a temporary file
                File.WriteAllText(sourceFile, code);

                // Run the Python script using the Python interpreter
                ProcessStartInfo startInfo = new ProcessStartInfo("python");
                startInfo.Arguments = $"\"{sourceFile}\"";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using (Process process = Process.Start(startInfo)) {
                    result.OutputMessage = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0) {
                        // Script execution succeeded
                        result.Success = true;
                    } else {
                        // Script execution failed, read the error messages
                        result.Success = false;
                        List<string> errorMessages = new List<string>();
                        string line;
                        using (StreamReader reader = process.StandardError) {
                            while ((line = reader.ReadLine()) != null) {
                                errorMessages.Add(line);
                            }
                        }
                        result.ErrorMessages = errorMessages;
                    }
                }
            } catch (Exception ex) {
                // Exception occurred during script execution
                result.Success = false;
                result.ErrorMessages = new List<string> { ex.Message };
            } finally {
                // Clean up temporary files
                if (File.Exists(sourceFile)) {
                    File.Delete(sourceFile);
                }
                if (File.Exists(outputFile)) {
                    File.Delete(outputFile);
                }
                if (File.Exists(errorFile)) {
                    File.Delete(errorFile);
                }
                Directory.Delete(tempDirectory);
            }

            return result;
        }
    }

    public class CSharpCompilerService {
        public CompilerResult CompileCode(string code) {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            string sourceFile = Path.Combine(tempDirectory, "temp.cs");
            string outputFile = Path.Combine(tempDirectory, "temp.exe");
            string errorFile = Path.Combine(tempDirectory, "errors.txt");

            CompilerResult result = new CompilerResult();

            try {
                // Save the code to a temporary file
                File.WriteAllText(sourceFile, code);

                // Compile the code using csc
                ProcessStartInfo startInfo = new ProcessStartInfo("csc");
                startInfo.Arguments = $"/out:\"{outputFile}\" \"{sourceFile}\"";
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using (Process process = Process.Start(startInfo)) {
                    process.WaitForExit();

                    if (process.ExitCode == 0) {
                        // Compilation succeeded
                        result.Success = true;

                        ProcessStartInfo executeStartInfo = new ProcessStartInfo(outputFile);
                        executeStartInfo.RedirectStandardOutput = true;
                        executeStartInfo.UseShellExecute = false;
                        executeStartInfo.CreateNoWindow = true;

                        using (Process executeProcess = Process.Start(executeStartInfo)) {
                            result.OutputMessage += executeProcess.StandardOutput.ReadToEnd();
                            executeProcess.WaitForExit();
                        }
                    } else {
                        // Compilation failed, read the error messages
                        result.Success = false;
                        List<string> errorMessages = new List<string>();
                        string line;
                        using (StreamReader reader = process.StandardError) {
                            while ((line = reader.ReadLine()) != null) {
                                errorMessages.Add(line);
                            }
                        }
                        result.ErrorMessages = errorMessages;
                    }
                }
            } catch (Exception ex) {
                // Exception occurred during compilation
                result.Success = false;
                result.ErrorMessages = new List<string> { ex.Message };
            } finally {
                // Clean up temporary files
                if (File.Exists(sourceFile)) {
                    File.Delete(sourceFile);
                }
                if (File.Exists(outputFile)) {
                    File.Delete(outputFile);
                }
                if (File.Exists(errorFile)) {
                    File.Delete(errorFile);
                }
                Directory.Delete(tempDirectory);
            }

            return result;
        }
    }

    public class CodespacesController : Controller {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly CppCompilerService _compilerServiceCpp;

        private readonly PythonCompilerService _compilerServicePy;

        private readonly CSharpCompilerService _compilerServiceCSh;

        public CodespacesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _compilerServiceCpp = new CppCompilerService();
            _compilerServicePy =  new PythonCompilerService();
            _compilerServiceCSh = new CSharpCompilerService();
        }

        
        public IActionResult Show(int id, string? output) {
            Codespace codespace = db.Codespaces.FirstOrDefault(c => c.AnswerId == id);
            if (codespace == null) {
                return NotFound();
            }
            ViewBag.Output = output;
            return View(codespace);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int id, string? output) {

            Codespace codespace = db.Codespaces.FirstOrDefault(c => c.AnswerId == id);
            Answer answer = db.Answers.FirstOrDefault(c => c.Id == id);

            if (answer == null) {
                return NotFound();
            }
            
            if (!(answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))) {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui cod care nu va apartine";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }


            if (codespace == null) {
                return NotFound();
            }
            ViewBag.Uanswer = answer.UserId;
            ViewBag.Output = output;
            return View(codespace);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult EditReal(int id, string code, string language) {
            Codespace codespace = db.Codespaces.FirstOrDefault(c => c.AnswerId == id);
            Answer answer = db.Answers.FirstOrDefault(c => c.Id == id);
            if (answer == null || codespace == null) {
                return NotFound();
            }

            if (!(answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))) {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui cod care nu va apartine";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }

            codespace.Content = code;
            codespace.Language = language;
            db.SaveChanges();

            return RedirectToAction("Edit", "Codespaces",  new { id = answer.Id });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult addCodespace(int discussionId, string code, string language) {
            var sanitizer = new HtmlSanitizer();

            Answer answer = new Answer {DiscussionId = discussionId};
            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            answer.Content = code.Replace("<br>", "\n");
            answer.IsCode = true;

            if (ModelState.IsValid) {

                db.Answers.Add(answer);
                db.SaveChanges();

                Codespace codespace = new Codespace {
                    AnswerId = answer.Id,
                    DiscussionId = discussionId,
                    Content = code,
                    Language = language
                };

                db.Codespaces.Add(codespace);
                db.SaveChanges();

                Discussion discussion = db.Discussions.Include("User")
                        .Where(dis => dis.Id == answer.DiscussionId)
                        .First();

                Notification NewNotification = new Notification {
                    Read = false,
                    DateMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
                    DateDay = DateTime.Now.Day,
                    UserId = discussion.UserId,
                    DiscussionId = answer.DiscussionId,
                    AnswerId = answer.Id,
                    Type = 1
                };

                // incrementeaza nr de notificari necitite ale user-ului
                discussion.User.UnreadNotifications++;

                db.Notifications.Add(NewNotification);
                db.SaveChanges();

  

                TempData["message"] = "Codespace created successfully";
                TempData["messageType"] = "alert-success";
            } else {

                foreach (var modelState in ModelState.Values) {
                    foreach (var error in modelState.Errors) {
                        // You can log or output the error message
                        Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                TempData["message"] = "Invalid model state";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Show", "Discussions", new { id = discussionId });
        }


        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult New(int discussionId, string code = "", string? output = "", string? language = "") {
            Codespace codespace = new Codespace();
            codespace.DiscussionId = discussionId;
            language ??= "cpp";
            output ??= "";
            codespace.Language = language;
            codespace.Content = Uri.UnescapeDataString(code);
            ViewBag.Output = Uri.UnescapeDataString(output);
            return View(codespace);
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult RunForm(Codespace codespace, string action) {
            if (action == "Run") {
                return RedirectToAction("CompileCode", "Codespaces", new { discussionId = codespace.DiscussionId, code = codespace.Content, language = codespace.Language });
            } else if (action == "AddAnswer") {
                return RedirectToAction("addCodespace", "Codespaces", new { discussionId = codespace.DiscussionId, code = codespace.Content, language = codespace.Language });
            } else if (action == "RunCC") {
                return RedirectToAction("CompileCodeCC", "Codespaces", new { answerId = codespace.AnswerId, code = codespace.Content, language = codespace.Language });
            } else  if (action == "AddAnswerCC") {
                return RedirectToAction("EditReal", "Codespaces", new { id = codespace.AnswerId, code = codespace.Content, language = codespace.Language });
            }

            return BadRequest("No valid choice!");
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult CompileCode(int discussionId, string code, string language) {
            CompilerResult result;

            if (!ModelState.IsValid) {
                return RedirectToAction("Edits", "Codespaces", new { discussionId = discussionId, language = language });
            }

            switch (language) {
                case "cpp":
                    result = _compilerServiceCpp.CompileCode(code);
                    break;
                case "python":
                    result = _compilerServicePy.CompileCode(code);
                    break;
                case "csharp":
                    result = _compilerServiceCSh.CompileCode(code);
                    break;
                default:
                    return BadRequest("Invalid language specified.");
            }

            if (result.Success) {
                string output = "no result!";
                if (result.OutputMessage != null)
                    output = result.OutputMessage;
  
                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/New?discussionId={discussionId}&code={encodedCode}&output={encodedOutput}&language={language}");
            } else {
                string output = "no result!";
                foreach (var errorMessage in result.ErrorMessages) {
                    output += HtmlEncoder.Default.Encode(errorMessage) + "\n";
                }

                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/New?discussionId={discussionId}&code={encodedCode}&output={encodedOutput}&language={language}");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult CompileCodeRun(int answerId, string code, string language) {
            CompilerResult result;

            switch (language) {
                case "cpp":
                    result = _compilerServiceCpp.CompileCode(code);
                    break;
                case "python":
                    result = _compilerServicePy.CompileCode(code);
                    break;
                case "csharp":
                    result = _compilerServiceCSh.CompileCode(code);
                    break;
                default:
                    return BadRequest("Invalid language specified.");
            }

            if (result.Success) {
                string output = "no result!";
                if (result.OutputMessage != null)
                    output = result.OutputMessage;

                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/Show?id={answerId}&output={encodedOutput}");
            } else {
                string output = "no result!";
                foreach (var errorMessage in result.ErrorMessages) {
                    output += HtmlEncoder.Default.Encode(errorMessage) + "\n";
                }

                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/Show?id={answerId}&output={encodedOutput}");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult CompileCodeCC(int answerId, string code, string language) {
            CompilerResult result;

            switch (language) {
                case "cpp":
                    result = _compilerServiceCpp.CompileCode(code);
                    break;
                case "python":
                    result = _compilerServicePy.CompileCode(code);
                    break;
                case "csharp":
                    result = _compilerServiceCSh.CompileCode(code);
                    break;
                default:
                    return BadRequest("Invalid language specified.");
            }

            if (result.Success) {
                string output = "no result!";
                if (result.OutputMessage != null)
                    output = result.OutputMessage;

                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/Edit?id={answerId}&output={encodedOutput}");
            } else {
                string output = "no result!";
                foreach (var errorMessage in result.ErrorMessages) {
                    output += HtmlEncoder.Default.Encode(errorMessage) + "\n";
                }

                string encodedCode = Uri.EscapeDataString(code);
                string encodedOutput = Uri.EscapeDataString(output);
                return Redirect($"/Codespaces/Edit?id={answerId}&output={encodedOutput}");
            }
        }
    }
}
