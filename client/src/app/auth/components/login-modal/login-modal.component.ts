import {Component} from '@angular/core';
import {AlertService} from "../../../shared/services/alert.service";
import {AuthService} from "../../services/auth.service";
import {ModalService} from "../../../shared/services/modal.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {FormValidators} from "../../../shared/validators/FormValidators";

@Component({
  selector: 'app-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.scss']
})
export class LoginModalComponent {

  constructor(
    public alertService: AlertService,
    private authService: AuthService,
    private modalService: ModalService
  ) {
  }

  userName = new FormControl('', [
    Validators.required,
    // Validators.minLength(3)
  ]);

  password = new FormControl(
    '',
    [
      Validators.required,
      // Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/gm)
    ]
  );

  loginForm = new FormGroup({
    userName: this.userName,
    password: this.password,
  });

  login() {
    this.alertService.alertShown = true;
    this.alertService.alertMsg = 'Please wait, you are logging in';
    this.alertService.alertColor = 'blue';

    this.authService
      .authenticate({userName: this.userName.value!, password: this.password.value!})
      .subscribe({
        next: res => {
          this.alertService.alertColor = 'green';
          this.alertService.alertMsg = 'Logged in successfully.';

          setTimeout(() => {
            this.alertService.alertShown = false;
            this.modalService.toggleModal('login', false);
            this.authService.login(res.token, this.userName.value!);
          }, 1000);
        },
        error: _ => {
          this.alertService.alertColor = 'red';
          this.alertService.alertMsg = 'Logging in failed.';
        }
      });
  }
}
