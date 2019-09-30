import {Component, OnInit, ViewChild} from '@angular/core';
import {MdDialogRef, MdSnackBar} from '@angular/material';
import {ProfileService} from '../profile.service';
import {Bounds, CropperSettings, ImageCropperComponent} from 'ng2-img-cropper';
import {PersonModel} from '../../shared/models/person-model';
import {PersonService} from "../../person/person.service";

@Component({
  selector: 'app-change-profile-picture-dialog',
  templateUrl: './change-profile-picture-dialog.component.html',
  styleUrls: ['./change-profile-picture-dialog.component.css']
})
export class ChangeProfilePictureDialogComponent implements OnInit {
  name: string;
  data1: any;
  cropperSettings1: CropperSettings;
  croppedWidth: number;
  croppedHeight: number;

  profile: PersonModel;

  @ViewChild('cropper', undefined) cropper: ImageCropperComponent;

  constructor(private profileService: ProfileService,
              public dialog: MdDialogRef<ChangeProfilePictureDialogComponent>,
              public snackBar: MdSnackBar,
  private personService: PersonService) {
    this.cropperSettings1 = new CropperSettings();
    this.cropperSettings1.width = 200;
    this.cropperSettings1.height = 200;

    this.cropperSettings1.croppedWidth = 200;
    this.cropperSettings1.croppedHeight = 200;

    this.cropperSettings1.canvasWidth = 500;
    this.cropperSettings1.canvasHeight = 450;

    this.cropperSettings1.minWidth = 10;
    this.cropperSettings1.minHeight = 10;

    this.cropperSettings1.rounded = true;
    this.cropperSettings1.keepAspect = true;
    this.cropperSettings1.fileType = 'img/png';
    this.cropperSettings1.noFileInput = true;

    this.cropperSettings1.cropperDrawSettings.strokeColor = 'rgba(255,255,255,1)';
    this.cropperSettings1.cropperDrawSettings.strokeWidth = 2;

    this.data1 = {};
  }

  ngOnInit() {
    this.profileService.getCurrentProfile().subscribe(result => {
      this.profile = result;
    });
    document.getElementById('custom-input').click();
  }

  cropped(bounds: Bounds) {
    this.croppedHeight = bounds.bottom - bounds.top;
    this.croppedWidth = bounds.right - bounds.left;
  }

  fileChangeListener($event) {
    const image: any = new Image();
    const file: File = $event.target.files[0];
    const myReader: FileReader = new FileReader();
    const that = this;
    myReader.onloadend = function (loadEvent: any) {
      image.src = loadEvent.target.result;
      that.cropper.setImage(image);
    };

    myReader.readAsDataURL(file);
  }

  saveProfilePicture() {
    // TODO bisschen hacky, sollte ich noch in schöner umbauen
    const picture = this.data1.image.split(',')[1];
    this.personService.updatePersonPicture(picture).subscribe(
      () => {
        this.snackBar.open('Profilbild gespeichert!', 'OK', {duration: 3000});
        this.dialog.close();
      },
      () => {
        this.dialog.close();
      });
  }
}
