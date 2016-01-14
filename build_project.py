#!/usr/bin/python

import os
import sys
import getopt

help = 'build_project.py -a <App ID>'
IOS_PROJECT = '/sendbird-sample/SendBirdiOSSample'
ANDROID_PROJECT = '/sendbird-sample/SendBirdAndroidSample'
WEB_PROJECT = "/sendbird-sample/SendBirdWebSample"
XAMARIN_DRIOD_PROJECT = "/sendbird-sample/SendBirdXamarinSample/Sample.Droid"

BASE_DIR = "."


def pull_source(base_dir):
  try:
    os.mkdir(base_dir)
  except:
    pass

  global BASE_DIR
  BASE_DIR = base_dir + "/sendbird-sample"
  print "Base Dir: {}".format(BASE_DIR)
  os.system("git clone https://github.com/smilefam/sendbird-sample " + BASE_DIR + "/sendbird-sample")


def main(argv):
  try:
    opts, args = getopt.getopt(argv, "ha:", ["app_id="])
  except getopt.GetoptError:
    print help
    sys.exit(2)

  for opt, arg in opts:
    if opt == '-h':
      print help
      sys.exit()
    elif opt in ("-a", "--app-id"):
      app_id = arg

  print "Application ID: {}".format(app_id)

  pull_source("./" + app_id)

  # Build iOS Project
  print "BASE_DIR: {}".format(BASE_DIR)
  ios_file = open(BASE_DIR + IOS_PROJECT + "/SendBirdiOSSample/ViewController.m")
  ios_file_content = ios_file.read()
  ios_file.close()

  ios_file_content = ios_file_content.replace("A7A2672C-AD11-11E4-8DAA-0A18B21C2D82", app_id)
  ios_output_file = open(BASE_DIR + IOS_PROJECT + "/SendBirdiOSSample/ViewController_tmp.m", "w")
  ios_output_file.write(ios_file_content)
  ios_output_file.close()

  os.remove(BASE_DIR + IOS_PROJECT + "/SendBirdiOSSample/ViewController.m")
  os.rename(BASE_DIR + IOS_PROJECT + "/SendBirdiOSSample/ViewController_tmp.m", BASE_DIR + IOS_PROJECT + "/SendBirdiOSSample/ViewController.m")

  # Build Android Project
  and_file = open(BASE_DIR + ANDROID_PROJECT + "/app/src/main/java/com/sendbird/android/sample/MainActivity.java")
  and_file_content = and_file.read()
  and_file.close()

  and_file_content = and_file_content.replace("A7A2672C-AD11-11E4-8DAA-0A18B21C2D82", app_id)
  and_output_file = open(BASE_DIR + ANDROID_PROJECT + "/app/src/main/java/com/sendbird/android/sample/MainActivity_tmp.java", "w")
  and_output_file.write(and_file_content)
  and_output_file.close()

  os.remove(BASE_DIR + ANDROID_PROJECT + "/app/src/main/java/com/sendbird/android/sample/MainActivity.java")
  os.rename(BASE_DIR + ANDROID_PROJECT + "/app/src/main/java/com/sendbird/android/sample/MainActivity_tmp.java", BASE_DIR + ANDROID_PROJECT + "/app/src/main/java/com/sendbird/android/sample/MainActivity.java")

  # Build Web Project
  web_file = open(BASE_DIR + WEB_PROJECT + "/static/js/common.js")
  web_file_content = web_file.read()
  web_file.close()

  web_file_content = web_file_content.replace("A7A2672C-AD11-11E4-8DAA-0A18B21C2D82", app_id)
  web_output_file = open(BASE_DIR + WEB_PROJECT + "/static/js/common_tmp.js", "w")
  web_output_file.write(web_file_content)
  web_output_file.close()

  os.remove(BASE_DIR + WEB_PROJECT + "/static/js/common.js")
  os.rename(BASE_DIR + WEB_PROJECT + "/static/js/common_tmp.js", BASE_DIR + WEB_PROJECT + "/static/js/common.js")

  # Xamarin Droid Project
  xroid_file = open(BASE_DIR + XAMARIN_DRIOD_PROJECT + "/MainActivity.cs")
  xroid_file_content = xroid_file.read()
  xroid_file.close()

  xroid_file_content = xroid_file_content.replace("A7A2672C-AD11-11E4-8DAA-0A18B21C2D82", app_id)
  xroid_output_file = open(BASE_DIR + XAMARIN_DRIOD_PROJECT + "/MainActivity_tmp.cs", "w")
  xroid_output_file.write(xroid_file_content)
  xroid_output_file.close()

  os.remove(BASE_DIR + XAMARIN_DRIOD_PROJECT + "/MainActivity.cs")
  os.rename(BASE_DIR + XAMARIN_DRIOD_PROJECT + "/MainActivity_tmp.cs", BASE_DIR + XAMARIN_DRIOD_PROJECT + "/MainActivity.cs")

  # Archive project
  os.system("tar zcvf " + "./sendbird-sample_" + app_id + ".tar.gz " + BASE_DIR)

  print "Finished."

if __name__ == "__main__":
  if len(sys.argv) > 1:
    main(sys.argv[1:])
  else:
    print help
