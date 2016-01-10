#!/bin/bash

# A POSIX variable
OPTIND=1

IOS_FILE_PATH="./SendBirdiOSSample/SendBirdiOSSample/ViewController.m"
ANDROID_FILE_PATH='./SendBirdAndroidSample/app/src/main/java/com/sendbird/android/sample/MainActivity.java'
WEB_FILE_PATH='./SendBirdWebSample/static/js/common.js'
XAMARIN_DRIOD_FILE_PATH='./SendBirdXamarinSample/Sample.Droid/MainActivity.cs'

function show_help {
  echo 'set_app_id.sh -a <App ID>'
}

while getopts "ha:" opt; do
  case "$opt" in
  h)
    show_help
    exit 0
    ;;
  a)
    app_id=$OPTARG
    ;;
  *)
    show_help
    exit 0
    ;;
  esac
done

echo "Your Appplication ID: $app_id"

# iOS Project
sed -i '' -e "s/A7A2672C-AD11-11E4-8DAA-0A18B21C2D82/$app_id/g" $IOS_FILE_PATH

# Android Project
sed -i '' -e "s/A7A2672C-AD11-11E4-8DAA-0A18B21C2D82/$app_id/g" $ANDROID_FILE_PATH

# Web Project
sed -i '' -e "s/A7A2672C-AD11-11E4-8DAA-0A18B21C2D82/$app_id/g" $WEB_FILE_PATH

# Xamarin Droid Project
sed -i '' -e "s/A7A2672C-AD11-11E4-8DAA-0A18B21C2D82/$app_id/g" $XAMARIN_DRIOD_FILE_PATH