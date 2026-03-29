export type TaskStatus = 'Pending' | 'Completed';

export type TaskPriority = 0 | 1 | 2; 
// or expand if backend uses more levels

export interface Task {
  id: string;
  userId: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  category: string | null;
  dueDate: string | null; // ISO string
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDto {
  title: string;
  description: string | null;
  priority: number;
  category: string | null;
  dueDate: string | null;
}

export interface UpdateTaskDto {
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: number;
  category: string | null;
  dueDate: string | null;
}