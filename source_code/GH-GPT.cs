using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Parameters;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.ComponentModel;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_5d8e2 : GH_ScriptInstance
{
  #region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
  #endregion

  #region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
  #endregion
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  #region Runscript
  private void RunScript(bool sendQuestion, string apiKey, string prompt, ref object A)
  {
    if (sendQuestion)
    {
      string messageContent = "hello";
      double temperature = 0.7;
      A = PostToGPT3(apiKey, messageContent, temperature);
    }
    else
    {
      A = "waiting to post";
    }

  }
  #endregion
  #region Additional

  // By Andycamos 2023/06/26
  // https://github.com/andycamos

  // Post Struct
  public class MessageGo
  {
    public string role { get; set; }
    public string content { get; set; }
  }

  public class OpenAIChatRequest
  {
    public string model { get; set; }
    public List<MessageGo> messages { get; set; }
    public double temperature { get; set; }
  }

  // Receive Struct
  public class Message
  {
    public string role { get; set; }
    public string content { get; set; }
  }

  public class Choice
  {
    public int index { get; set; }
    public Message message { get; set; }
    public string finish_reason { get; set; }
  }

  public class OpenAIChatResponse
  {
    public string id { get; set; }
    public string object_name { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<Choice> choices { get; set; }
  }

  private static readonly HttpClient client = new HttpClient();
  public string PostToGPT3(string apiKey, string messageContent, double temperature)
  {
    var requestObject = new OpenAIChatRequest
      {
        model = "gpt-3.5-turbo",
        messages = new List<MessageGo>
        {
          new MessageGo { role = "user", content = messageContent }
          },
        temperature = temperature
        };

    string jsonString = JsonConvert.SerializeObject(requestObject);

    var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

    client.DefaultRequestHeaders.Remove("Authorization");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

    var response = client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent).Result;

    if (response.IsSuccessStatusCode)
    {
      var responseContent = response.Content.ReadAsStringAsync().Result;

      OpenAIChatResponse responseObject = JsonConvert.DeserializeObject<OpenAIChatResponse>(responseContent);

      if (responseObject.choices.Count > 0)
      {
        return responseObject.choices[0].message.content;
      }
      else
      {
        return "No response from assistant";
      }
    }
    else
    {
      return string.Format("Request failed with status code: {0}", response.StatusCode);
    }
  }
  #endregion
}