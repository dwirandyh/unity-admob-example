using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : MonoBehaviour
{

    private BannerView bannerView;
    private InterstitialAd interstitial;

    AndroidJavaObject currentActivity;

    public string appID = "";
    public string bannerID = "";
    public string interstitialID = "";

    private string toastString = "";



    private void Awake()
    {
#if UNITY_ANDROID
        string app = appID;
#elif UNITY_IPHONE
            string app = "ca-app-pub-3940256099942544~1458002511";
#else
            string app = "unexpected_platform";
#endif

        MobileAds.Initialize(app);
    }

    private void Start()
    {
        RequestInterstitial();
    }

    public void ShowBanner()
    {
        RequestBanner();
    }

    public void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string banner = bannerID;
#elif UNITY_IPHONE
        string banner = bannerIOS;
#else
        string banner = "unexpected_platform";
#endif

        bannerView = new BannerView(banner, AdSize.Banner, AdPosition.Bottom);

        // banner event
        bannerView.OnAdLoaded += BannerOnLoaded;
        bannerView.OnAdFailedToLoad += BannerOnFailedToLoad;
        bannerView.OnAdOpening += BannerOnOpened;
        bannerView.OnAdClosed += BannerOnAdClosed;
        bannerView.OnAdLeavingApplication += BannerOnLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);

        // banner ads event

    }

    // Banner delegate
    private void BannerOnLoaded(object sender, System.EventArgs args){
        if (Application.platform == RuntimePlatform.Android)
        {
            showToastOnUiThread("Handle Ad Loaded");
        }
    }

    private void BannerOnFailedToLoad(object sender, AdFailedToLoadEventArgs args){
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    private void BannerOnOpened(object sender, System.EventArgs args){
        MonoBehaviour.print("HandleAdOpened event received");
    }

    private void BannerOnAdClosed(object sender, System.EventArgs args){
        MonoBehaviour.print("HandleAdClosed event received");
    }

    private void BannerOnLeavingApplication(object sender, System.EventArgs args){
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }




    private void RequestInterstitial(){
        interstitial = new InterstitialAd(interstitialID);
        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();

        interstitial.LoadAd(request);
    }

    // interstitial delegate
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void MyShowToastMethod()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            showToastOnUiThread("It Worked!");
        }
    }

    void showToastOnUiThread(string toastString)
    {
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        this.toastString = toastString;

        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(showToast));
    }

    void showToast()
    {
        Debug.Log("Running on UI thread");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
        AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }
}
