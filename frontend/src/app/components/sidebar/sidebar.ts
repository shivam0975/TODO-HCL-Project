import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent {
  readonly userName$;
  readonly firstLetter$;
  readonly initials$;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    this.userName$ = this.authService.user$.pipe(
      map(user => (user?.fullName || 'User').trim() || 'User')
    );

    this.firstLetter$ = this.userName$.pipe(
      map(name => name.charAt(0).toUpperCase())
    );

    this.initials$ = this.userName$.pipe(
      map(name =>
        name
          .split(' ')
          .filter(Boolean)
          .map((part: string) => part[0])
          .join('')
          .toUpperCase()
          .slice(0, 2) || 'U'
      )
    );
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
