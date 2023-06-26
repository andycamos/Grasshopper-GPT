#  By Andycamos 2023/06/26
#  https://github.com/andycamos


import clr

clr.AddReference('System.Web.Extensions')
from System.Web.Script.Serialization import JavaScriptSerializer
from System.Net import *
from System import Convert
from System.Text import Encoding
import urllib  # - this needs Python 2.7.7 or later!

if sendQuestion:
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;  # TLS 1.2

    # Create a web client
    client = WebClient()

    # OpenAI API Key
    api_key = apiKey

    temperature = 0.7

    # Set up headers
    client.Headers[HttpRequestHeader.Authorization] = "Bearer " + api_key
    client.Headers[HttpRequestHeader.ContentType] = "application/json"

    # Data to send
    data_dict = {
        "model": "gpt-3.5-turbo",
        "messages": [
            {
                "role": "user",
                "content": prompt
            }
        ],
        "temperature": temperature,
    }
    serializer = JavaScriptSerializer()
    data = serializer.Serialize(data_dict)

    # Request URL
    address = "https://api.openai.com/v1/chat/completions"

    # Send request and print response
    response = client.UploadString(address, data)

    # Try to parse the response
    try:
        # Deserialize the response
        response_dict = serializer.DeserializeObject(response)

        # Get the content from the response
        a = response_dict["choices"][0]["message"]["content"]

    except KeyError:
        # Handle missing key in the response
        print("Error: The response from the OpenAI API is missing some expected keys.")
    except IndexError:
        # Handle missing index in the response
        print("Error: The response from the OpenAI API does not contain any choices.")
    except Exception as e:
        # Handle any other exceptions
        print("An error occurred: " + str(e))
else:
    a = 'waiting to post'

