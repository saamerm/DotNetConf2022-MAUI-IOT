using Newtonsoft.Json;

namespace WheresMyChai;

public partial class MainPage : ContentPage
{
    string _deviceId = "e00fce6827199403b3da9372"; //This is your Particle Device Id
    string AccessToken = "e8de4549fe9f11d10720bbfef5f0aa924d4aad79";
    Label _resultLabel = new Label()
    {
        HorizontalOptions = LayoutOptions.CenterAndExpand
    };

    public MainPage()
    {
        var ChangeLedCommandOn = new Command<string>(LedOn);
        var ChangeLedCommandOff = new Command<string>(LedOff);
        var onButton = new Button { Text = "Full", Command = ChangeLedCommandOn, Margin = 10 };
        var offButton = new Button { Text = "Empty", Command = ChangeLedCommandOff, Margin = 10 };
        Title = "Where's My Tea";
        Content = new StackLayout
        {
            VerticalOptions = LayoutOptions.CenterAndExpand,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            Children = {
                    new Label {
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = "Welcome to the World's First Essential Tea Notifier App!"
                    },
                    onButton,
                    offButton,
                    _resultLabel
                }
        };
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await GetTeaStatus();
    }
    #region Private Functions
    private void LedOn(string changeValue = "on")
    {
        changeValue = "on";
        string accessToken = AccessToken; //This is your Particle Cloud Access Token
        string partilceFunc = "led"; //This is the name of your Particle Function

        HttpClient client = new HttpClient
        {
            BaseAddress =
            new Uri("https://api.particle.io/")
        };

        var content = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("access_token", accessToken),
                new KeyValuePair<string, string>("args", changeValue )
            });

        var result = client.PostAsync("v1/devices/" + _deviceId + "/" + partilceFunc, content);
        _resultLabel.TextColor = Colors.Blue;
        _resultLabel.Text = "The Tea is here!";
        ResultLabelUpdate(1);
    }
    private void LedOff(string changeValue = "off")
    {
        changeValue = "off";
        string accessToken = AccessToken; //This is your Particle Cloud Access Token
        string partilceFunc = "led"; //This is the name of your Particle Function

        HttpClient client = new HttpClient
        {
            BaseAddress =
            new Uri("https://api.particle.io/")
        };

        var content = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("access_token", accessToken),
                new KeyValuePair<string, string>("args", changeValue )
            });

        var result = client.PostAsync("v1/devices/" + _deviceId + "/" + partilceFunc, content);
        ResultLabelUpdate(0);
    }
    private void ResultLabelUpdate(int status = -1)
    {
        if (status == 1)
        {
            _resultLabel.TextColor = Colors.Blue;
            _resultLabel.Text = "The Tea is here!";
            BackgroundColor = Colors.SkyBlue;
        }
        else if (status == 0)
        {
            _resultLabel.TextColor = Colors.Red;
            _resultLabel.Text = "Rats! The Tea is gone again";
            BackgroundColor = Colors.LightGray;
        }
        else
        {
            _resultLabel.Text = "Tea Status: Unknown";
            BackgroundColor = Colors.LightYellow;
        }
    }
    private async Task GetTeaStatus()
    {
        string accessToken = AccessToken; //This is your Particle Cloud Access Token
        HttpClient client = new HttpClient();
        var url = string.Format("https://api.particle.io/v1/devices/{0}/led?access_token={1}", _deviceId, AccessToken);
        var content = new StringContent("");
        var json = await client.PostAsync(url, content);
        var result = JsonConvert.DeserializeObject<ParticleResponse>(await json.Content.ReadAsStringAsync());
        ResultLabelUpdate(result.return_value);
    }
    #endregion
}

public class ParticleResponse
{
    public string id { get; set; }
    public string name { get; set; }
    public int return_value { get; set; }
    public bool connected { get; set; }
}