import {Component, OnInit} from '@angular/core';
import {ModalService} from "../../shared/services/modal.service";
import {AuthService} from "../../auth/services/auth.service";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit{

  isAuthenticated = false;


  constructor(
    public modalService: ModalService,
    public authService: AuthService
  ) {
  }

  ngOnInit() {
    this.authService.isAuthenticatedSubject.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });
  }

  logout() {
    this.authService.logout();
  }


}
