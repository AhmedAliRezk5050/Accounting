import { Component } from '@angular/core';
import {AuthService} from "../../auth/services/auth.service";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  isAuthenticated = false;

  constructor(
    public authService: AuthService
  ) {
  }

  ngOnInit() {
    this.authService.isAuthenticatedSubject.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });
  }

  visibleSubMenus: {[key: string]: boolean} = {
    users: false,
    invoices: false
  };

  toggleSubMenu(menu: string): void {
    this.visibleSubMenus[menu] = !this.visibleSubMenus[menu];
  }
}
