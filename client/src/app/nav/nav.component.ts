import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { error } from 'protractor';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};  

  constructor(public accountService : AccountService,
    private router : Router,
    private toastr: ToastrService) { 
  }

  ngOnInit(): void {
  }

  logIn() {
    this.accountService.login(this.model).subscribe(response=>{      
      this.router.navigateByUrl('/members');
    },error=>{
      this.toastr.error(error.error);      
      console.log(error);
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  } 
}


//loggedIn: boolean = false;
// logIn() {
  //   this.accountService.login(this.model).subscribe(response=>{      
  //     this.loggedIn = true;
  //     console.log(response);
  //   },error=>{
  //     console.log(error);
  //   });
  // }

  // logout() {
  //   this.accountService.logout();
  //   this.loggedIn = false;
  // }

  // getCurrentUser() { 
  //   this.accountService.currentUser$.subscribe(user => {
  //     this.loggedIn = !! user;
  //   },error => {
  //     console.log(error);
  //   });
  // }