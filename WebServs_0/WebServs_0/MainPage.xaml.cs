using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WebServs_0
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        private string url = "https://jsonplaceholder.typicode.com/posts";
        private HttpClient _client = new HttpClient();
        // item source for ListView: ObservableCollection
        private ObservableCollection<Post> _posts;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            // get list of all posts
            var content = await _client.GetStringAsync(url);
            // content is a string: need to deserialize it
            var posts = JsonConvert.DeserializeObject<List<Post>>(content);

            _posts = new ObservableCollection<Post>(posts);
            postsListView.ItemsSource = _posts;
            

            base.OnAppearing();
        }

        async void OnAdd(object sender, System.EventArgs e)
        {
            var post = new Post { Title = "Title " + DateTime.Now.Ticks };

            // show in ListView right away assuming it will post on server ok
            _posts.Insert(0, post);  // add to ListView

            // serialize Post object intoa string = http content
            var content = JsonConvert.SerializeObject(post);
            // to create a post on the server: create a http post request
            await _client.PostAsync(url, new StringContent(content));

            // line not executed until above call returns from posting to server
            //_posts.Insert(0, post);  // add to ListView
        }

        async void OnUpdate(object sender, System.EventArgs e)
        {  // nothing happens on UI: we have not implemented the INotifyPropertyChanged interface
            // but the call to the server succeeds
            var post = _posts[0];
            post.Title += "UPDATED";

            var content = JsonConvert.SerializeObject(post);
            await _client.PostAsync(url + "/" + post.Id, new StringContent(content));
        }

        async void OnDelete(object sender, System.EventArgs e)
        {
            var post = _posts[0];
            _posts.Remove(post);  // before call to server to see changes immediately
            await _client.DeleteAsync(url + "/" + post.Id);
        }
    }
}
