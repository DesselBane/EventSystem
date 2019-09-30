import {Component, OnInit} from '@angular/core';
import {ProfileService} from '../profile.service';
import {MdDialog} from '@angular/material';
import {ChangeProfilePictureDialogComponent} from '../profile-image-dialog/change-profile-picture-dialog.component';
import {PersonModel} from '../../shared/models/person-model';
import {PersonService} from '../../person/person.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profile: PersonModel = new PersonModel('', '', '', 0);
  editMode = false;
  profilepicture: any;
  personImage: String = '../../../assets/img/city_nuernberg.png';
  private personService: PersonService;


  constructor(private profileService: ProfileService, public dialog: MdDialog, personService: PersonService) {
    this.personService = personService;
  }

  ngOnInit() {
    this.loadProfile();
  }

  save() {
    this.profileService.updateCurrentProfile(this.profile).subscribe(
      () => this.editMode = false,
      () => {
        this.editMode = false;
      });
  }

  showChangeProfilePictureDialog() {
    const dialog = this.dialog.open(ChangeProfilePictureDialogComponent);
    dialog.afterClosed().toPromise().then(() => {
      this.loadProfile();
    });

  }

  switchEditMode() {
    this.editMode = !this.editMode;
  }


  private loadPersonImage() {
    this.personService.getPersonPicture(this.profile.id).subscribe((result) => {
     this.personImage = 'data:image/png;base64,'+result;
    });
  }

  private loadProfile() {
    this.profileService.getCurrentProfile().subscribe((result) => {
      this.profile = result;
      this.loadPersonImage();
    });
  }



}

